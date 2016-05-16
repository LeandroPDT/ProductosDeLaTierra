using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;
using System.Web.WebPages;

namespace Site.Controllers {
    [Authorize]
    public class NotificacionController : ApplicationController {
        public ActionResult Nuevas() {
            var VM = new ListarNotificacionViewModel();
            VM.CalcResultado();
            return PartialView(VM);
        }
        public ActionResult Probar() {
            for (int i = 0; i < 30; i++) {
                Notificacion.Notificar(Sitio.Usuario.UsuarioID, "Prueba de notificaciones. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus ut quam sapien, id dictum purus. Ut vel consectetur justo.", "/");    
            }
            TempData["InfoMessage"] = "Notificacion de prueba generada con exito";
            return Redirect("/");
        }

    }
}

namespace Site.Models {
    public class ListarNotificacionViewModel {
        public List<Notificacion> Resultado { get; set; }

        public void CalcResultado() {
            var db = DbHelper.CurrentDb();
            var sql = Notificacion.BaseQuery(50);
            sql.Append("WHERE Notificacion.UsuarioID = @0", Sitio.Usuario.UsuarioID);
            sql.Append("ORDER BY Fecha DESC");
            Resultado = db.Fetch<Notificacion>(sql);
            // ya lo actualizo porque el tipo las vió
            db.Execute("UPDATE Notificacion set Leido = 1 where Leido = 0 AND Notificacion.UsuarioID = @0", Sitio.Usuario.UsuarioID);
        }
    }
}