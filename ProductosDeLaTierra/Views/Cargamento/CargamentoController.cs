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
    public class CargamentoController : Controller
    {
        const Seguridad.Permisos Permiso = Seguridad.Permisos.Cargamento;
        //
        // GET: /Cargamentos/
		[CustomAuthorize(Roles = Permiso)]
        public ActionResult Index(IndexCargamentoViewModel form) {
            try {
                var VM = new IndexCargamentoViewModel();
                TryUpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex) {
                ModelState.AddModelError("all", ex.Message);
                form.Resultado = new List<Models.Cargamento>();
                return View(form);
            }
	    }
           
            
        [HttpGet]
		[CustomAuthorizeBorrar(Roles = Permiso)]
        public ActionResult Borrar(int id) {
            var db = DbHelper.CurrentDb();
            var VM = Cargamento.SingleOrDefault(id);
            return View(VM);
        }


        [HttpPost]
		[CustomAuthorizeBorrar(Roles = Permiso)]
        public ActionResult Borrar(int id, FormCollection form) {
            try {
                Cargamento cargamento = Cargamento.SingleOrDefault(id);
                if (cargamento.CurrentUserCanAccessToFunction( Seguridad.Feature.Borrar)) {
                    cargamento.Delete();
                }
                
                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Registro borrado con éxito";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex) {
                if (Request.IsAjaxRequest()) {
                    return Content("Error: " + DbHelper.EnCastellanoPorFavor(ex));
                }
                else {
                    TempData["ErrorMessage"] = "Error: " + DbHelper.EnCastellanoPorFavor(ex);
                    return View();
                }
            }
        }
         
        
		[CustomAuthorize(Roles = Permiso)]
        public ActionResult MasInfo(int id) {
            var Cargamento = Models.Cargamento.SingleOrDefault(id);
            return PartialView(Cargamento);
        }
        
		[CustomAuthorize(Roles = Permiso)]
        public ActionResult ExtraInfo(int id) {
            var VM = Models.Cargamento.SingleOrDefault(id);
            return PartialView("MasInfo", VM);
        }

        public ActionResult VerMercaderia(int id) {
            var VM = Models.Cargamento.SingleOrDefault(id);
            return PartialView("VerMercaderia", VM);
        }

        public ActionResult Liquidacion(int id) {
            var VM = Models.Cargamento.SingleOrDefault(id);
            return PartialView("Liquidacion", VM);
        }

        /* por ahora no necesito seleccionar cargamentos individualmente
        [NoCache]
        public JsonResult Lista(string term, string TipoVenta, string Estado) {
            var retval = new List<IDNombrePar>();
            var db = DbHelper.CurrentDb();
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT TOP 25 CargamentoID as ID, Cargamento.Referencia as Nombre,  'Raza: '+ Substring( (SELECT Nombre FROM Raza WHERE Cargamento.RazaID=Raza.RazaID) , 1, 50)"+ ( new Cargamento.EstadoCargamento(Estado).implicaActivo? "+ ' - Dias de vida: ' + CONVERT(char, CONVERT(int, CONVERT(DATETIME, '"+DateTime.Today.ToMMDDYYYY()+"') - FechaNacimiento ))": "") +" as ExtraInfo");
            sql.Append("FROM Cargamento");
            sql.Append("Where 1=1");
            Cargamento.AppendTipoVentaMatching(sql, TipoVenta);
            Cargamento.AppendEstadoMatching(sql, Estado);
            if (!term.IsEmpty())
                sql.AppendKeywordMatching(term, "Cargamento.Referencia");
            sql.Append("ORDER BY Cargamento.Referencia");            
            retval = db.Query<IDNombrePar>(sql).ToList();
            return Json(retval, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult ListaValidar(string Numero, string Genero, string Estado) {
            if (Numero.AsInt() > 0) {
                var db = DbHelper.CurrentDb();
                var sql = PetaPoco.Sql.Builder;
                sql.Append("SELECT TOP 1 CargamentoID as ID, Numero as Nombre");
                sql.Append("FROM Cargamento");
                sql.Append("Where Cargamento.Numero = @0", Numero);
                Cargamento.AppendGeneroMatching(sql, Genero);
                Cargamento.AppendEstadoMatching(sql, Estado);

                var retval = db.Query<IDNombrePar>(sql);
                return Json(retval, JsonRequestBehavior.AllowGet);
            }
            else {
                var retval = new List<IDNombrePar>();
                return Json(retval, JsonRequestBehavior.AllowGet);
            }
            
        }
         * */
        private ViewResult AccessDeniedView() {
            return new ViewResult { ViewName = Request.IsAjaxRequest() ? "~/Views/Shared/_AccessDeniedAjax.cshtml" : "~/Views/Shared/AccessDenied.cshtml" };
        }

    }
    

}
