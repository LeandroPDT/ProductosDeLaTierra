using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using Site.Models;

namespace Site.Areas.Cobranzas.Controllers
{
    [Authorize]
    public class CobranzaController : Controller
    {
        const Seguridad.Permisos Permiso = Seguridad.Permisos.EventoCobro;
        //
        // GET: 
		[CustomAuthorize(Roles = Permiso)]
        public ActionResult Index(IndexCobranzaViewModel form) {
            try {
                var VM = new IndexCobranzaViewModel();
                TryUpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex) {
                ModelState.AddModelError("all", ex.Message);
                form.Resultado = new List<Models.Cobranza>();
                return View(form);
            }
	    }

        [CustomAuthorize(Roles=Permiso)]
        public ActionResult Nuevo() {
            Cobranza VM = new Cobranza();
            // si es empleado de la empresa tendra que elegir el usuario, sino el usuario es el actual.
            VM.ProveedorID = Sitio.EsEmpleado ? VM.ProveedorID : Sitio.Usuario.ProveedorID.IsEmpty()?Sitio.Usuario.UsuarioID:Sitio.Usuario.ProveedorID??0;
            if (Request.IsAjaxRequest())
                return PartialView("Editar", VM);
            else
                return View("Editar", VM);
            
        }

                
		[CustomAuthorize(Roles = Permiso)]
        public ActionResult Editar(int id) {
            Cobranza VM = Cobranza.SingleOrDefault(id);
            if (Request.IsAjaxRequest())
                return PartialView("Editar", VM);
            else
                return View("Editar", VM);
        }

        [HttpPost]
		[CustomAuthorizeEditar(Roles = Permiso)]
        public ActionResult Editar(Cobranza form, string actionType) {
            TryUpdateModel(form);
            if (ModelState.IsValid && form.IsValid(ModelState) /*&& ValidateCobranza(form,ModelState)*/) {
                var db = DbHelper.CurrentDb();
                Cobranza rec;
                if (form.CobranzaID != 0) {
                    rec = Cobranza.SingleOrDefault(form.CobranzaID);                    
                }
                else {
                    rec = new Cobranza();
                };
                TryUpdateModel(rec);
                var Tipo = new EventoTipo("Cobro");
                if (ModelState.IsValid && rec.CanUpdate(ModelState) && rec.IsValid(ModelState)) {
                    try {
                        bool DebeNotificarse = rec.IsNew() && Tipo.RolNotificable != null && Tipo.RolNotificable != "";
                        rec.DoSave();
                        if (DebeNotificarse)
                            rec.notify(ModelState);

                        if (Request.IsAjaxRequest()) {
                            return Content("OK");
                        } else {
                            TempData["InfoMessage"] = "Registro guardado con éxito";
                            return RedirectToAction("Index");
                        }
                    } catch (Exception ex) {
                        ModelState.AddModelError("", ex.Message);
                    }
                    if (Request.IsAjaxRequest()) {
                        return Content("OK");
                    } else {
                        TempData["InfoMessage"] = "Registro guardado con éxito";
                        return RedirectToAction((actionType == "Guardar" ? "Editar" : (actionType == "Guardar y nuevo" ? "Nuevo" : "Index")), new { id = rec.CobranzaID });
                    }
                }
            }
            if (Request.IsAjaxRequest())
                return Content("Error " + ModelState.ToHTMLString());
            else
                return View("Editar", form);
        }
        
    
        [HttpGet]
		[CustomAuthorizeBorrar(Roles = Permiso)]
        public ActionResult Borrar(int id) {
            var db = DbHelper.CurrentDb();
            var Cobranza = db.SingleOrDefault<Cobranza>(id);
            return View(Cobranza);
        }


        [HttpPost]
		[CustomAuthorizeBorrar(Roles = Permiso)]
        public ActionResult Borrar(int id, FormCollection form) {
            try {
                Cobranza cobranza = Cobranza.SingleOrDefault(id);
                if (cobranza!=null) {
                    if (cobranza.CurrentUserCanAccessToFunction(Seguridad.Feature.Borrar)) {
                        cobranza.Delete();
                    }

                    if (Request.IsAjaxRequest()) {
                        return Content("OK");
                    } else {
                        TempData["InfoMessage"] = "Registro borrado con éxito";
                        return RedirectToAction("Index");
                    }
                } else {

                    TempData["ErrorMessage"] = "Error: No existe la cobranza especificada";
                    return View();
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
         
        
    }

}
