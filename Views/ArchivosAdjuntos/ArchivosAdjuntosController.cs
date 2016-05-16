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
    public class ArchivosAdjuntosController : ApplicationController {
        [NoCache]
        public ActionResult Galeria(Entidad e) {
            return PartialView(e);
        }
        
        [NoCache]
        public ActionResult GaleriaFotos(Entidad e) {
            return PartialView(e);
        }

        public ActionResult Subir(Entidad e, HttpPostedFileBase file1  ) {
            try {	        
                if (e.ID.IsEmpty() || e.Nombre.IsEmpty()) {
                    //return Content("Error: No se indicó la entidad a la que se debe adjuntar el archivo");
                    throw new ApplicationException("No se indicó la entidad a la que se debe adjuntar el archivo");
                }
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
                    var Folder = Server.MapPath("/content/subidos/" + e.Nombre + "/");
                    if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                    file.SaveAs(Folder + FinalFileName);
                    
                    //agrego el archivo a la base
                    var aa = new ArchivoAdjunto();
                    aa.NombreArchivo = FinalFileName;
                    aa.Entidad = e.Nombre;
                    aa.ID = e.ID;
                    aa.Fecha = DateTime.Now;
                    aa.Titulo = sFileName;
                    DbHelper.CurrentDb().Save(aa);

                    return Content("OK");
                }
                else {
                    //return Content("Error: No indico el archivo a subir");
                    throw new ApplicationException("No indico el archivo a subir");
                }
		
	        }
	        catch (Exception) {
		        //return Content("Error: " + ex.Message);
                throw;
	        }

        }

        [HttpPost]
        public ActionResult Borrar(int id) {
            try {
                var db = DbHelper.CurrentDb();
                var aa = ArchivoAdjunto.SingleOrDefault(id);
                if (aa != null) {
                    var file = Server.MapPath(aa.PathAndFile);
                    if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
                    db.Delete(aa);
                }

                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Regimen borrado con exito";
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


    }
}
