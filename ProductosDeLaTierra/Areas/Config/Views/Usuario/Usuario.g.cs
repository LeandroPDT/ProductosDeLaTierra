using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Site.Models
{
    [TableName("Usuario")]
    [PrimaryKey("UsuarioID")]
    [ExplicitColumns]
    public class Usuario {
        public const string AvatarFolder = "/content/subidos/usuario/";
        public const int MaxInvalidPasswordAttempts = 3;
        public const int PasswordAttemptWindow = 5;
        public const int MinRequiredPasswordLength = 8;

        [Column("UsuarioID")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "UsuarioID")]
        public int UsuarioID { get; set; }

        [Column("UserName")]
        [Display(Name = "Usuario")]
        [Required]
        public String UserName { get; set; }

        [Column("Password")]
        [Display(Name = "Contraseña")]
        public String Password { get; set; }

        [Column("Nombre")]
        [Display(Name = "Nombre")]
        [Required]
        public String Nombre { get; set; }

        [Column("EMail")]
        [Display(Name = "EMail")]
        [Required]
        public String Email { get; set; }

        [Column("Activo")]
        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notas { get; set; }

        [Column("Avatar")]
        [ReadOnly(true)]
        public String Avatar { get; set; }

        [Column("AvatarChico")]
        [ReadOnly(true)]
        public String AvatarChico { get; set; }

		[PetaPoco.Column("IsLockedOut")]
		[Display(Name = "Bloqueado")]
		public Boolean IsLockedOut { get; set; }

		[PetaPoco.Column("LastLockedOutDate")]
		[DataType(DataType.DateTime)]
        [ReadOnly(true)]
		public DateTime? LastLockedOutDate { get; set; }

		[PetaPoco.Column("FailedPasswordAttemptCount")]
		[Required(ErrorMessage = "Requerido")]
        [ReadOnly(true)]
		public int FailedPasswordAttemptCount { get; set; }

		[PetaPoco.Column("FailedPasswordAttemptWindowStart")]
		[DataType(DataType.DateTime)]
        [ReadOnly(true)]
		public DateTime? FailedPasswordAttemptWindowStart { get; set; }

		[PetaPoco.Column("PasswordResetToken")]
        [ReadOnly(true)]
		public Guid? PasswordResetToken { get; set; }

		[PetaPoco.Column("PasswordResetExpiration")]
		[DataType(DataType.DateTime)]
        [ReadOnly(true)]
		public DateTime? PasswordResetExpiration { get; set; }

        [PetaPoco.Column("LastLoginDate")]
        [DataType(DataType.DateTime)]
        [ReadOnly(true)]
        public DateTime? LastLoginDate { get; set; }

        [Column("LastLoginIP")]
        [ReadOnly(true)]
        public String LastLoginIP { get; set; }

       
		[PetaPoco.Column("Comision")]
		[Display(Name = "Comisión (%)")]
		[DataType(DataType.Currency)]
        [Required]
		public double Comision { get; set; }
        

        [Column("ProveedorID")]
        [Display(Name = "Proveedor Administrado")]
        public int? ProveedorID { get; set; }

        [ResultColumn]
        [Display(Name = "Proveedor Administrado")]
        public String Proveedor { get; set; }

        public Usuario() {
            Avatar = "nn.gif";
            AvatarChico = "nn50x50.gif";
            Activo = true;
            Comision = 0;
        }


        public string PasswordResetLink {
            get {
                return "/Account/ResetPassword/" + PasswordResetToken.ToString();
            }
        }

        public static Usuario SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE usuario.UsuarioID = @0", id);
            return DbHelper.CurrentDb().SingleOrDefault<Usuario>(sql);
        }

        public static Usuario SingleOrDefault(string login) {
            var sql = BaseQuery();
            sql.Append("WHERE usuario.UserName = @0", login);
            return DbHelper.CurrentDb().SingleOrDefault<Usuario>(sql);
        }

        
        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
            sql.AppendSelectTop(TopN);
            sql.Append("Usuario.*, Proveedor.Nombre as Proveedor");
            sql.Append("FROM Usuario");
            sql.Append("LEFT JOIN Usuario Proveedor on Proveedor.UsuarioID= Usuario.ProveedorID");
            return sql;
        }


        private IList<UsuarioRol> _Items;
        public IList<UsuarioRol> Items {
            get {
                if (_Items == null) {
                    var sql = UsuarioRol.BaseQuery();
                    sql.Append("where UsuarioRol.UsuarioID = @0", this.UsuarioID);
                    sql.Append("ORDER BY Rol.Nombre");
                    _Items = DbHelper.CurrentDb().Fetch<UsuarioRol>(sql);
                }
                return _Items;
            }
            set {
                _Items = value;
            }
        }

        public void InitItems(int minSlots = 5, int minFreeSlots = 2) {
            //me aseguro que la minimamente la cantidad que quiero libre
            for (int i = Items.Count(); i < minSlots; i++) {
                Items.Add(new UsuarioRol());
                minFreeSlots--;
            }
            //me aseguro que haya algunos lugar libres
            for (int i = 0; i < minFreeSlots; i++) {
                Items.Add(new UsuarioRol());
            }
        }

        public bool IsValid(ModelStateDictionary ModelState) {
            if (DbHelper.CurrentDb().ExecuteScalar<int>("SELECT count(*) From Usuario where UsuarioID <> @0 and UserName = @1", this.UsuarioID, this.UserName) > 0) {
                ModelState.AddModelError("All", "El nombre de usuario ya fue usado por otra persona");
                return false;
            }
            ProveedorID.ZeroToNull();
            if (!ProveedorID.IsEmpty() && !Usuario.HasRol(ProveedorID ?? 0, "Proveedor")) { 
                ModelState.AddModelError("ProveedorID", "El usuario administrado no es un proveedor");
                return false;
            }
            return true;
        }
        public override string ToString() {
            if (this.UsuarioID == 0) {
                return "Nuevo Usuario";
            }
            else {
                return "Usuario " + Nombre;
            }
        }

        public static bool ValidateUser(string username, string password) {
            var db = DbHelper.CurrentDb();
            var u = db.SingleOrDefault<Usuario>("Select TOP 1 * from Usuario where UserName = @0", username);
            if (u == null) {
                throw new ApplicationException("Usuario o contraseña incorrecto");
            }
            else if (!u.Activo) {
                throw new ApplicationException("Usuario inactivo");
            }
            else if (u.IsLockedOut) {
                throw new ApplicationException("Usuario bloqueado. Contactese con el administrador del sistema para solicitar el desbloqueo de su cuenta.");
            }
            else if (u.Password != GetHash(password)) {
                UpdateFailureCount(u);
                throw new ApplicationException("Usuario o contraseña incorrecto");
            }
            else {
                UpdateSuccesfulLogon(u);
                return true;
            }

        }

        private static void UpdateFailureCount(Usuario u) {
            DateTime windowEnd = (u.FailedPasswordAttemptWindowStart ?? DateTime.Now).AddMinutes(PasswordAttemptWindow);
            int failureCount = u.FailedPasswordAttemptCount;
            var db = DbHelper.CurrentDb();

            if (failureCount == 0 || DateTime.Now > windowEnd) {
                // First password failure or outside of PasswordAttemptWindow. 
                // Start a new password failure count from 1 and a new window starting now.
                db.Execute("UPDATE Usuario SET FailedPasswordAttemptCount = @0, FailedPasswordAttemptWindowStart = @1 Where UsuarioID = @2", 1, DateTime.Now, u.UsuarioID);
            }
            else {
                if (failureCount++ >= MaxInvalidPasswordAttempts) {
                    db.Execute("UPDATE Usuario SET IsLockedOut = 1, LastLockedOutDate = @0 Where UsuarioID = @1", DateTime.Now, u.UsuarioID);
                }
                else {
                    db.Execute("UPDATE Usuario SET FailedPasswordAttemptCount = @0 Where UsuarioID = @1", failureCount, u.UsuarioID);
                }
            }
        }

        private static void UpdateSuccesfulLogon(Usuario u) {
            var db = DbHelper.CurrentDb();
            var sql = PetaPoco.Sql.Builder;
            sql.Append("UPDATE Usuario");
            sql.Append("SET LastLoginDate = @0", DateTime.Now);
            sql.Append(", LastLoginIP = @0", System.Web.HttpContext.Current.Request.UserHostAddress);
            sql.Append(", FailedPasswordAttemptCount = 0");
            sql.Append("Where UsuarioID = @0", u.UsuarioID);
            db.Execute(sql);
        }


        public static string GetHash(string value) {
            //para que usar FormsAuthentication.HashPasswordForStoringInConfigFile() si podemos obtener un hash usando directamente  Cryptography
            //http://blog.veggerby.dk/2008/07/06/abuse-of-formsauthenticationhashpasswordforstoringinconfigfile-method/
            var algorithm = System.Security.Cryptography.MD5.Create();
            byte[] data = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));
            string md5 = "";
            for (int i = 0; i < data.Length; i++) {
                md5 += data[i].ToString("x2").ToLowerInvariant();
            }
            return md5;
        }

        public static bool HasRol(int id, string rol) {
            return (from IDNombrePar par in RolUserIDList() where par.ID == id && par.Nombre==rol select par).ToList().Count > 0;            
        }

        public static List<IDNombrePar> RolUserIDList() {            
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT Usuario.UsuarioID as ID, rol.Nombre as Nombre");
            sql.Append("FROM Usuario");
            sql.Append("INNER JOIN UsuarioRol usuarioRol ON Usuario.UsuarioID = usuarioRol.UsuarioID");
            sql.Append("INNER JOIN Rol rol ON usuarioRol.RolID = rol.RolID");
            return DbHelper.CurrentDb().Fetch<IDNombrePar>(sql);  
        }


    }
}
