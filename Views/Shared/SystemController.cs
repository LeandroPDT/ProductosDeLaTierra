using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace Site.Controllers {
    public class SystemController : Controller {

        public ActionResult Ping() {
            return Content(DateTime.Now.ToString());
        }

        public ActionResult CausarError() {
            var zero = 0;
            var e = 1 / zero;
            return Content(DateTime.Now.ToString());
        }

        public ActionResult TestEmail() {
            try {
                var smtp = new System.Net.Mail.SmtpClient();
                var email = new System.Net.Mail.MailMessage();
                email.To.Add(new System.Net.Mail.MailAddress("leandropalma@live.com.ar", "webmaster"));
                email.Subject = "Test de envio de mail";
                email.Body = "Test de envio de mail";
                email.IsBodyHtml = false;
                smtp.Send(email);
            }
            catch (Exception ex) {
                return Content("Error: " + ex.Message);
            }
            return Content("OK");
        }

        public ActionResult MantenimientoNocturno() {
            var db = DbHelper.CurrentDb();
            // borro los temporales
            //db.Execute("delete from re_equipo_obra");

            return Content("OK");
        }

        public ActionResult EnviarNotificacionesPendientes() {
            var Desde = DateTime.Now.AddMinutes(-15);
            var smtp = new System.Net.Mail.SmtpClient();
            var db = DbHelper.CurrentDb();
            var sql = Notificacion.BaseQuery();
            sql.Append("WHERE Fecha >= @0", Desde);
            sql.Append("AND Leido = 0");
            sql.Append("ORDER BY Fecha");
            var Notificaciones = db.Fetch<Notificacion>(sql);
            if (Notificaciones.Count > 0) {
                var Usuarios = db.Fetch<Usuario>("WHERE Usuario.UsuarioID IN (@0)", (from rec in Notificaciones select rec.UsuarioID).Distinct().ToList());
                foreach (var u in Usuarios) {
                    try {
                        var email = new System.Net.Mail.MailMessage();
                        email.To.Add(new System.Net.Mail.MailAddress(u.Email, u.Nombre));
                        email.Subject = String.Format("Notificaciones de Productos De La Tierra pendientes");
                        email.Body = this.RenderViewToString("EmailNotificacion", (from rec in Notificaciones where rec.UsuarioID == u.UsuarioID select rec).ToList());
                        email.IsBodyHtml = true;
                        smtp.Send(email);
                    }
                    catch (Exception) {
                        // me la como y sigo...
                    }
                }
            }
            return Content("OK");
        }


	}
}
