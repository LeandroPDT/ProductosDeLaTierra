using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using Site.Models;

namespace Site.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        //
        // GET: /Reportes/
		[CustomAuthorize(Roles = Seguridad.Permisos.ReporteActividad)]
        public ActionResult Index() {
            return RedirectToAction("ReporteActividad");
	    }

        
		[CustomAuthorize(Roles = Seguridad.Permisos.ReporteActividad)]
        public ActionResult ReporteActividad(ReporteActividad form) {
            try {
                var VM = new ReporteActividad();
                TryUpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex) {
                ModelState.AddModelError("all", ex.Message);
                form.Resultado = new List<Models.ReporteActividad.ReporteActividadItem>();
                return View(form);
            }
	    }

        
		[CustomAuthorize(Roles = Seguridad.Permisos.Liquidacion)]
        public ActionResult Liquidacion(int id) {
            var cargamento = Cargamento.SingleOrDefault(id);
            var VM = new Liquidacion(id);
            if (VM!=null && cargamento.Recibido && Seguridad.CanAccess(Seguridad.Permisos.Liquidacion) && (Sitio.EsEmpleado||cargamento.HasUser(Sitio.Usuario.UsuarioID)||cargamento.HasUser(Sitio.Usuario.ProveedorID??0)))
                return View("Liquidacion", VM);
            else 
                return AccessDeniedView();
        }

        private ViewResult AccessDeniedView() {
            return new ViewResult { ViewName = Request.IsAjaxRequest() ? "~/Views/Shared/_AccessDeniedAjax.cshtml" : "~/Views/Shared/AccessDenied.cshtml" };
        }
                       
    }

}
