using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using Site.Models;
using System.Reflection;

namespace Site.Areas.Eventos.Controllers{
	[Authorize]
    public class EventoController : ApplicationController {
        // GET: /Cargamentos/
        public ActionResult Index(IndexEventoViewModel form) {
            try {
                var VM = new IndexEventoViewModel();
                TryUpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex) {
                ModelState.AddModelError("all", ex.Message);
                form.Resultado = new List<IndexEventoViewModel.ListarEventoViewModel>();
                return View(form);
            }
	    }
        
        [HttpGet]
        public ActionResult Nuevo(string type,int? id) {
            var Tipo = new EventoTipo(type);
            Cargamento cargamento = id.IsEmpty()? new Cargamento(){ProveedorID = Sitio.EsEmpleado? 0:Sitio.Usuario.UsuarioID} :  Cargamento.SingleOrDefault(id??0) ;
            if (cargamento.IsNew() || UserCanAccessToFunctionOverObject(Tipo.Permiso,Seguridad.Feature.Editar,cargamento)) {
                Evento VM = new Evento(){TipoEventoID=Tipo.ID, CargamentoID = id, Cargamento = cargamento, Mercaderia= cargamento.Mercaderia.clone()};
                VM.TipoEventoID = Tipo.ID;
                if (Request.IsAjaxRequest())
                    return PartialView(Tipo.ViewName, VM);
                else
                    return View(Tipo.ViewName, VM);
            }
            else {
                return AccessDeniedView();
            }
            
        }
        /*
        [HttpGet]
        public ActionResult Editar(string type, int? id) {
            if (id.IsEmpty()) {
                ModelState.AddModelError("", "Debe indicar un ID de Cargamento válido");
                return Content("Error " + ModelState.ToHTMLString());
            }
            var Tipo = new EventoTipo(type);
            Cargamento cargamento = Cargamento.SingleOrDefault(id??0);
            if (UserCanAccessToFunctionOverObject(Tipo.Permiso,Seguridad.Feature.Entrar,cargamento)) {
                var VM = Evento.SingleOrDefault(Tipo.ID, id??0);       
                if (Request.IsAjaxRequest())
                    return PartialView(Tipo.ViewName, VM);
                else
                    return View(Tipo.ViewName, VM);
            }
            else {
                return AccessDeniedView();
            }
        }
        */
        
        [HttpGet]
        public ActionResult Editar(string type, int? id) {
            if (id.IsEmpty()) {
                ModelState.AddModelError("", "Debe indicar un ID de Cargamento válido");
                return Content("Error " + ModelState.ToHTMLString());
            }
            var VM = Evento.SingleOrDefault(id??0)?? new Evento();
            Cargamento cargamento = VM.Cargamento;
            var Tipo = new EventoTipo(VM.TipoEventoID);            
            if (UserCanAccessToFunctionOverObject(Tipo.Permiso,Seguridad.Feature.Entrar,cargamento)) {
                if (Request.IsAjaxRequest())
                    return PartialView(Tipo.ViewName, VM);
                else
                    return View(Tipo.ViewName, VM);
            }
            else {
                return AccessDeniedView();
            }
        }

        [HttpPost]
        public ActionResult Editar(Evento form, string actionType) {
            var Tipo = new EventoTipo(form.TipoEventoID);
            if (!UserCanAccessToFunctionOverObject(Tipo.Permiso,Seguridad.Feature.Editar,form.Cargamento)) {
                return AccessDeniedView();
            }            
            Evento rec;
            if (form.EventoID > 0 ) {
                rec = Evento.SingleOrDefault(form.EventoID);
            }
            else{
                rec = new Evento();
            }
            TryUpdateModel(rec);
            if (ModelState.IsValid && rec.CanUpdate(ModelState) && rec.IsValid(ModelState)) {
                try {
                    bool DebeNotificarse = rec.IsNew() && Tipo.RolNotificable != null && Tipo.RolNotificable != ""; 
                    rec.Save();
                    if (DebeNotificarse)
                        rec.notify(ModelState);
                    rec.notifyIfExistsDiffrence(ModelState);
                    
                    if (Request.IsAjaxRequest()) {
                        return Content("OK");
                    }
                    else {
                        TempData["InfoMessage"] = "Registro guardado con éxito";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex) {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            if (Request.IsAjaxRequest())
                return Content("Error " + ModelState.ToHTMLString());
            else
                return View(Tipo.ViewName, form);
        }

        
        [HttpPost]
        public ActionResult Borrar(string type, int? id) {
            try {
                if (id.IsEmpty()) {
                    ModelState.AddModelError("", "Debe indicar un ID de Cargamento válido");
                    return Content("Error: " + ModelState.ToHTMLString());
                }
                var EventoABorrar = Evento.SingleOrDefault(id??0)?? new Evento();
                var Tipo = new EventoTipo(EventoABorrar.TipoEventoID);            
                if (Seguridad.CanDelete(Tipo.Permiso) && EventoABorrar.CanUpdate(ModelState) && UserCanAccessToFunctionOverObject(Tipo.Permiso,Seguridad.Feature.Borrar,  EventoABorrar.Cargamento)) {
                    EventoABorrar.Delete();
                
                    if (Request.IsAjaxRequest()) {
                        return Content("OK");
                    }
                    else {
                        TempData["InfoMessage"] = "Registro borrado con éxito";
                        return RedirectToAction("Index");
                    }
                }
                else {
                    return Content("Error: " + ModelState.ToHTMLString());
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
             
        
        // este es el upload comun de archivos
        [AcceptVerbs(HttpVerbs.Post)]
        public object Upload(int id, HttpPostedFileBase file1) {

            try {
                int EventoID = BizLibMVC.Utiles.TryCInt(id);

                //subimos las fotos
                var file = Request.Files[0];
                if (file.ContentLength > 0) {
                    string sFileName = file.FileName;
                    //chequeo el tipo
                    string sFileType = System.IO.Path.GetExtension(sFileName).ToLower().Trim();
                    var permitidos = ".jpg ;.jpeg ;.gif ;.png ;.txt ;.doc ;.docx ;.xls ;.xlsx ;.ppt ;.pptx ;.pdf ";
                    if (string.IsNullOrEmpty(sFileType) || !permitidos.Contains(sFileType)) {
                        throw new ApplicationException("No se pudo subir archivo " + sFileName + ". Solo se permiten archivos con extensión " + permitidos);
                    }
                    var FinalFileName = BizLibMVC.Utiles.GetRandomPasswordUsingGUID(32) + sFileType;
                    var Folder = Server.MapPath("/content/subidos/Evento/");
                    if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                    file.SaveAs(Folder + FinalFileName);

                    return Content("OK");
                }
                else {
                    throw new HttpException(999, "No se pudo agregar el archivo. No se envió en el archivo");
                }
            }
            catch (Exception ex) {
                throw new HttpException(999, "No se pudo agregar el archivo. Error: " + ex.Message);
            }

        }



        
        // podrá gestionar si tiene el permiso y, o es empleado o participa del evento (con el rol que puede editarlo).
        private bool UserCanAccessToFunctionOverObject(int passID,Seguridad.Feature function, Cargamento obj) {
            return (Seguridad.CanAccessToFunction(Sitio.Usuario.UsuarioID,passID,function) && (Sitio.EsEmpleado || obj.HasUser(Sitio.Usuario.UsuarioID)));
        }

        private ViewResult AccessDeniedView() {
            return new ViewResult { ViewName = Request.IsAjaxRequest() ? "~/Views/Shared/_AccessDeniedAjax.cshtml" : "~/Views/Shared/AccessDenied.cshtml" };
        }

        


	}
}