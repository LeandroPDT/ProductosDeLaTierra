using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;
using System.Web.WebPages;
using System.Text;

namespace Site.Controllers {

    [Authorize]
    public class AyudaController : ApplicationController {

        public ActionResult Index() {
            var VM = new ListarAyudaViewModel();
            VM.CalcResultado();
            return View(VM);
        }

        [HttpPost]
        public ActionResult Index(ListarAyudaViewModel form) {

            try {
                var VM = new ListarAyudaViewModel();
                UpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex) {
                ModelState.AddModelError("all", ex.Message);
                form.Resultado = new List<Ayuda>();
                return View(form);
            }

        }


        public ActionResult Para(string id) {
            var VM = Ayuda.SingleOrDefault(id);
            if (VM == null) {
                VM = new Ayuda();
                VM.Titulo = id;
                VM.Codigo = id;
                VM.Notas = "-- Sin información -- ";
                DbHelper.CurrentDb().SaveAndLog(VM);
            }
            return Redirect("/ayuda/ver/" + VM.AyudaID.ToString());
        }

        public ActionResult Ver(int id) {
            var VM = Ayuda.SingleOrDefault(id);
            return View(VM);
        }

        public ActionResult Editar(int id) {
            var VM = Ayuda.SingleOrDefault(id);
            return View(VM);
        }

        [HttpPost]
        public ActionResult Editar(Ayuda form, string actionType) {
            if (ModelState.IsValid && form.IsValid(ModelState)) {
                var db = DbHelper.CurrentDb(); 
                Ayuda rec = null;
                // nunca viene vacío porque al crearlo por primera vez ya lo crea en base de datos
                if (!form.AyudaID.IsEmpty()) {
                    rec = Ayuda.SingleOrDefault(form.AyudaID);
                    UpdateModel(rec);
                }

                using (var scope = db.GetTransaction()) {
                    //pongo algunos valores por default
                    db.SaveAndLog(rec);
                    scope.Complete();
                }

                TempData["InfoMessage"] = "Ayuda guardada con exito";
                return Redirect("/Ayuda/ver/" + rec.AyudaID.ToString());
            }
            return View("Editar", form);
        }

        [HttpGet]
        public ActionResult Borrar(int id) {
            var db = DbHelper.CurrentDb();
            var Ayuda = db.SingleOrDefault<Ayuda>(id);
            return View(Ayuda);
        }


        [HttpPost]
        public ActionResult Borrar(int id, FormCollection form) {
            try {
                var db = DbHelper.CurrentDb();
                db.Execute("DELETE FROM AyudaItem WHERE AyudaID = @0; DELETE FROM Ayuda WHERE AyudaID = @0", id);
                db.LogDelete(typeof(Ayuda), id);

                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Ayuda borrado con exito";
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

        public ActionResult GenerarExcel() {
            ListarAyudaViewModel VM = new ListarAyudaViewModel();
            VM.q = "";
            VM.CantidadPorPagina = 0;
            VM.CalcResultado();

            dynamic ee = new BizLibMVC.ExcelExporter("Ayuda", BizLibMVC.ExcelExporter.Column.GetList(new BizLibMVC.ExcelExporter.Column("Nombre", "Nombre", BizLibMVC.ExcelExporter.ColumnType.StringCol, 100), new BizLibMVC.ExcelExporter.Column("Email", "email", BizLibMVC.ExcelExporter.ColumnType.StringCol, 100), new BizLibMVC.ExcelExporter.Column("Tipo", "AQue", BizLibMVC.ExcelExporter.ColumnType.StringCol, 50), new BizLibMVC.ExcelExporter.Column("Fecha Ayuda", "DateOpened", BizLibMVC.ExcelExporter.ColumnType.DateTimeCol, 50)), VM.Resultado);
            ee.WriteToResponse(System.Web.HttpContext.Current);
            return null;
        }



        // este es el upload de imagenes de redactorJs
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadImagen(int id, HttpPostedFileBase FileData) {

            try {
                int AyudaID = BizLibMVC.Utiles.TryCInt(id);

                //subimos las fotos
                var file = Request.Files["file"];
                if (file.ContentLength > 0) {
                    string sFileName = file.FileName;
                    //chequeo el tipo
                    string sFileType = System.IO.Path.GetExtension(sFileName).ToLower().Trim();
                    var permitidos = ".jpg ;.jpeg ;.gif ;.png ;";
                    if (string.IsNullOrEmpty(sFileType) || !permitidos.Contains(sFileType)) {
                        throw new ApplicationException("No se pudo subir archivo " + sFileName + ". Solo se permiten archivos con extensión " + permitidos);
                    }
                    var FinalFileName = id.ToString() + "_" + BizLibMVC.Utiles.GetRandomPasswordUsingGUID(32) + sFileType;
                    var Folder = Server.MapPath("/content/subidos/Ayuda/");
                    if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                    file.SaveAs(Folder + FinalFileName);

                    dynamic retval = new {filelink = "/content/subidos/Ayuda/" + FinalFileName};

                    return Json(retval);
                }
                else {
                    throw new HttpException(999, "No se pudo agregar el archivo. No se envió en el archivo");
                }
            }
            catch (Exception ex) {
                throw new HttpException(999, "No se pudo agregar el archivo. Error: " + ex.Message);
            }

        }

        // este es el upload comun de archivos
        [AcceptVerbs(HttpVerbs.Post)]
        public object Upload(int id, HttpPostedFileBase file1) {

            try {
                int AyudaID = BizLibMVC.Utiles.TryCInt(id);

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
                    var Folder = Server.MapPath("/content/subidos/Ayuda/");
                    if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                    file.SaveAs(Folder + FinalFileName);

                    AyudaItem wa = new AyudaItem();
                    wa.Archivo = FinalFileName;
                    wa.ArchivoNombre = sFileName;
                    wa.Fecha = DateTime.Now;
                    wa.US_ID = Sitio.Usuario.UsuarioID;
                    wa.AyudaID = AyudaID;
                    DbHelper.CurrentDb().Save(wa);
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



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Comentar(string id, FormCollection form) {
            if (form["Mensaje"].IsEmpty()) {
                TempData["InfoMessage"] = "No comentaste nada";
                return Redirect("/Ayuda/ver/" + id);
            }

            int AyudaID = BizLibMVC.Utiles.TryCInt(id);

            try {
                AyudaItem com = new AyudaItem();
                com.AyudaID = AyudaID;
                com.Mensaje = form["Mensaje"];
                com.Fecha = DateTime.Now;
                com.US_ID = Sitio.Usuario.UsuarioID;
                DbHelper.CurrentDb().Save(com);
                TempData["InfoMessage"] = "Comentario agregado con éxito";
                return Redirect("/Ayuda/ver/" + id + "/#comentarios");
            }
            catch (Exception ex) {
                TempData["ErrorMessage"] = "No se pudo guardar el comentario. Error: " + ex.Message;
                return Redirect("/Ayuda/ver/" + id);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BorrarComentario(string id, FormCollection form) {
            try {
                int ComentarioID = BizLibMVC.Utiles.TryCInt(id);
                dynamic Comentario = AyudaItem.SingleOrDefault(ComentarioID);
                if (Seguridad.CanAccess((int)Seguridad.Permisos.ModificarAyuda) || Comentario.US_ID == Sitio.Usuario.UsuarioID) {
                    DbHelper.CurrentDb().Execute("DELETE FROM AyudaItem where AyudaItemID = @0", ComentarioID);
                }
                else {
                    throw new ApplicationException("Solo lo puede borrar su autor");
                }
                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Borrado con éxito";
                    return Redirect("/Ayuda/ver/" + Comentario.AyudaID);
                }
            }
            catch (Exception ex) {
                if (Request.IsAjaxRequest()) {
                    return Content("Error: " + ex.Message);
                }
                else {
                    TempData["ErrorMessage"] = "Error: " + ex.Message;
                    return Redirect("/Ayuda/");
                }
            }
        }


    }
}
namespace Site.Models {
    public class ListarAyudaViewModel {
        
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public bool ExcluirCerrados { get; set; }
        public bool SoloSubscriptos { get; set; }
        public bool SoloSinResolver { get; set; }
        public string Tipo { get; set; }

        public List<Ayuda> Resultado { get; set; }

        public ListarAyudaViewModel() {
            q = Sitio.GetPref("ListarAyuda-q", "");
            CantidadPorPagina = Sitio.GetPref("ListarAyuda-CantidadPorPagina", 50);
            ExcluirCerrados = Sitio.GetPref("ListarAyuda-ExcluirCerrados", true);
            SoloSubscriptos = Sitio.GetPref("ListarAyuda-SoloSubscriptos", false);
            SoloSinResolver = Sitio.GetPref("ListarAyuda-SoloSinResolver", Sitio.EsDeveloper());
            Tipo = Sitio.GetPref("ListarAyuda-Tipo", "");
        }

        public void SetPref() {
            Sitio.SetPref("ListarAyuda-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("ListarAyuda-q", q);
            Sitio.SetPref("ListarAyuda-ExcluirCerrados", ExcluirCerrados);
            Sitio.SetPref("ListarAyuda-SoloSubscriptos", SoloSubscriptos);
            Sitio.SetPref("ListarAyuda-SoloSinResolver", SoloSinResolver);
            Sitio.SetPref("ListarAyuda-Tipo", Tipo);
        }

        public void CalcResultado() {
            var sql = Ayuda.BaseQuery(this.CantidadPorPagina);
            sql.Append("WHERE Ayuda.Titulo IS NOT NULL"); // por las dudas que haya cosas en blanco


            if (!this.Tipo.IsEmpty()) {
                sql.Append("AND Ayuda.Tipo = @0", this.Tipo);
            }


            if (this.ExcluirCerrados) {
                sql.Append("AND Ayuda.Cerrado = 0");
            }

            if (this.SoloSubscriptos) {
                sql.Append("AND (Ayuda.US_ID = @0 OR Exists (SELECT * from Ayudaubscripcion Where Ayudaubscripcion.US_ID = @0 and Ayudaubscripcion.AyudaID = Ayuda.AyudaID))", Sitio.Usuario.UsuarioID);
            }

            if (SoloSinResolver) {
                sql.Append("AND Ayuda.ResueltoDev = 0");
            }

            if (!this.q.IsEmpty())
                sql.AppendKeywordMatching(this.q, "Ayuda.Titulo", "Ayuda.Notas");

            sql.Append("ORDER BY Ayuda.Fecha");
            Resultado = DbHelper.CurrentDb().Fetch<Ayuda>(sql);
        }
    }
}
