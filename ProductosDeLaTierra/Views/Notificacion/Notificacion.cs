using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;
using System.Web.WebPages;
using DataAnnotationsExtensions;

namespace Site.Models {
    [TableName("Notificacion")]
    [PrimaryKey("NotificacionID")]
    [ExplicitColumns]
    public class Notificacion {
		[PetaPoco.Column("NotificacionID")]
		public int NotificacionID { get; set; }

		[PetaPoco.Column("Fecha")]
		[Required]
		[Display(Name = "Fecha")]
		[DataType(DataType.DateTime)]
		public DateTime Fecha { get; set; }

		[PetaPoco.Column("Texto")]
		[Display(Name = "Texto")]
		[StringLength(500)]
		public String Texto { get; set; }

        [PetaPoco.Column("Cuerpo")]
        [Display(Name = "Cuerpo")]
        public String Cuerpo { get; set; }

		[PetaPoco.Column("URL")]
		[Display(Name = "URL")]
		[StringLength(500)]
		public String URL { get; set; }


        [PetaPoco.Column("UsuarioID")]
		[Required]
		[Display(Name = "usuario")]
        public int UsuarioID { get; set; }

		[ResultColumn]
		public string Usuario { get; set; }


		[PetaPoco.Column("Leido")]
		[Required]
		[Display(Name = "Leido")]
		public Boolean Leido { get; set; }


        public string FullURL {
            get {
                return URL.StartsWith("http") ? URL : Utils.CombineURL(Sitio.URI, URL);
            }
        }

        public bool IsValid(ModelStateDictionary ModelState) {
            //if (DbHelper.CurrentDb().ExecuteScalar<int>("SELECT count(*) From EnsayoAro where EnsayoAroID <> @0 and OB_ID = @1 AND Numero = @2", this.EnsayoAroID, this.OB_ID, this.Numero) > 0) {
            //    ModelState.AddModelError("All", "Ya indicó ese numero de Aro para esa obra");
            //    return false;
            //}
            return true;
        }

        public bool CanUpdate(ModelStateDictionary ModelState) {
            return true;
        }

		public static Notificacion SingleOrDefault(int id) {
		    var sql = BaseQuery();
		    sql.Append("WHERE Notificacion.NotificacionID = @0", id);
		    return DbHelper.CurrentDb().SingleOrDefault<Notificacion>(sql);
		}
		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
		    sql.Append("Notificacion.*, usuario.Nombre as usuario");
		    sql.Append("FROM Notificacion");
            sql.Append("    INNER JOIN usuario ON Notificacion.UsuarioID = usuario.UsuarioID");
		    return sql;
		}

        public static void Notificar(int UsuarioID, string Texto, string URL, string Cuerpo = null) {
            var n = new Notificacion();
            n.UsuarioID = UsuarioID;
            n.Texto = Texto;
            n.URL = URL;
            n.Fecha = DateTime.Now;
            n.Cuerpo = Cuerpo;
            DbHelper.CurrentDb().Save(n);
        }

        public static void NotificarMasEmail(Usuario user, string Texto, string URL, string Cuerpo = null, string Extension = null) {
            Notificar(user.UsuarioID, Texto, URL, Cuerpo);
            var email = new System.Net.Mail.MailMessage();
            email.To.Add(user.Email);
            email.Body = Cuerpo ?? "" + Extension ?? "";
            email.Subject = Texto??"";
            email.IsBodyHtml = true;
            email.Send();
        }
        public static int NoLeidos() {
            return DbHelper.CurrentDb().Single<int>("SELECT COUNT(*) FROM Notificacion where Leido = 0 AND UsuarioID = @0", Sitio.Usuario.UsuarioID);
        }

	}
}
