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
            if (VM!=null && Seguridad.CanAccess(Seguridad.Permisos.Remanente) && (Sitio.EsEmpleado||VM.HasUser(Sitio.Usuario.UsuarioID)||VM.HasUser(Sitio.Usuario.ProveedorID??0)))
                return PartialView("VerMercaderia", VM);
            else 
                return AccessDeniedView();
        }

        [HttpGet]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Observaciones(int id)
        {
            var VM = Models.Cargamento.SingleOrDefault(id);
            return PartialView("Observaciones", VM);
        }


        [NoCache]
        public JsonResult Lista(string term, string Estado, int? ProveedorID)
        {
            var retval = new List<InfoCargamento>();
            var db = DbHelper.CurrentDb();
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT TOP 25 CargamentoID as ID, Cargamento.NumeroRemito as Nombre,  'Proveedor: '+ Proveedor.Nombre + ' Cliente: Cliente.Nombre' as ExtraInfo, Cargamento.Ganancia as Ganancia");
            sql.Append("FROM Cargamento");
            sql.Append("INNER JOIN Usuario Proveedor ON Proveedor.UsuarioID = Cargamento.ProveedorID");
            sql.Append("INNER JOIN Usuario Cliente ON Cliente.UsuarioID = Cargamento.ClienteID");
            sql.Append("Where 1=1");
            var estadoReference = new Cargamento();
            if (!Estado.IsEmpty()) {
                //estadoReference.Estado = Estado;                TODO
                estadoReference.Estado = "Vendido";
                sql.Append("AND Cargamento.Recibido = @0 AND Cargamento.Vendido = @1 AND Cargamento.Cobrado = @2", estadoReference.Recibido,estadoReference.Vendido, estadoReference.Cobrado);
            }
            if (!ProveedorID.IsEmpty())
            {
                sql.Append("AND Cargamento.ProveedorID = @0", ProveedorID ?? 0);
            }
            sql.AppendKeywordMatching(term, "Cargamento.NumeroRemito");
            sql.Append("ORDER BY Cargamento.NumeroRemito");
            retval = db.Query<InfoCargamento>(sql).ToList();
            return Json(retval, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult ListaValidar(string id, string Estado, int? ProveedorID)
        {
            var retval = new List<InfoCargamento>();
            if (!id.IsEmpty())
            {
                var db = DbHelper.CurrentDb();
                var sql = PetaPoco.Sql.Builder;
                sql.Append("SELECT TOP 1 CargamentoID as ID, Cargamento.NumeroRemito as Nombre,  'Proveedor: '+ Proveedor.Nombre + ' Cliente: Cliente.Nombre' as ExtraInfo, Cargamento.Ganancia as Ganancia");
                sql.Append("FROM Cargamento");
                sql.Append("INNER JOIN Usuario Proveedor ON Proveedor.UsuarioID = Cargamento.ProveedorID");
                sql.Append("INNER JOIN Usuario Cliente ON Cliente.UsuarioID = Cargamento.ClienteID");
                sql.Append("Where 1=1");
                if (!ProveedorID.IsEmpty())
                {
                    sql.Append("AND Cargamento.ProveedorID = @0", ProveedorID);
                }

                retval = db.Query<InfoCargamento>(sql).ToList();
                return Json(retval, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(retval, JsonRequestBehavior.AllowGet);
            }

        }


        private ViewResult AccessDeniedView() {
            return new ViewResult { ViewName = Request.IsAjaxRequest() ? "~/Views/Shared/_AccessDeniedAjax.cshtml" : "~/Views/Shared/AccessDenied.cshtml" };
        }


        public class InfoCargamento
        {
            public int ID { get; set; }
            public string Nombre { get; set; }
            public string ExtraInfo { get; set; }
            public double Ganancia { get; set; }
            public InfoCargamento() { }
            public InfoCargamento(int ID, string Nombre, string ExtraInfo)
            {
                this.ID = ID;
                this.Nombre = Nombre;
                this.ExtraInfo= ExtraInfo;
            }
        }

    }
    

}
