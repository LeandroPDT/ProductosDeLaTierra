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
            Notificar(user.UsuarioID, Cuerpo, URL);
            var email = new System.Net.Mail.MailMessage();
            email.To.Add(user.Email);
            email.Body =  (Extension ?? "") + "<br/><br/>Atentamente,<br/>Productos de la Tierra.<hr/><img src='data:image/jpeg;base64,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'/> ";
            email.Subject = Texto??"";
            email.IsBodyHtml = true;
            email.Send();
        }
        public static int NoLeidos() {
            return DbHelper.CurrentDb().Single<int>("SELECT COUNT(*) FROM Notificacion where Leido = 0 AND UsuarioID = @0", Sitio.Usuario.UsuarioID);
        }

	}
}
