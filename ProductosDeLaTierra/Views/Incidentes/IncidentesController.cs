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
    public class IncidentesController : ApplicationController {

        public ActionResult Index() {
            var VM = new ListarIncidentesViewModel();
            VM.CalcResultado();
            return View(VM);
        }

        [HttpPost]
        public ActionResult Index(ListarIncidentesViewModel form) {

            try {
                var VM = new ListarIncidentesViewModel();
                UpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex) {
                ModelState.AddModelError("all", ex.Message);
                form.Resultado = new List<Incidente>();
                return View(form);
            }

        }

        public ActionResult InformeCerrados(InformeIncidentesCerradosViewModel form) {
            var VM = new InformeIncidentesCerradosViewModel();
            TryUpdateModel(VM);
            VM.CalcResultado();
            VM.SetPref(); // guardo las preferencias para la próxima
            return View(VM);
        }


        public ActionResult InformeCerradosExcel() {
            var VM = new InformeIncidentesCerradosViewModel();
            //VM.CantidadPorPagina = 0;
            VM.CalcResultado();
            return new ExcelResult("Informes Incidentes cerrados", (IEnumerable<dynamic>)VM.Resultado, VM.Columns());

        }



        public ActionResult Ver(int id) {
            var VM = Incidente.SingleOrDefault(id);
            return View(VM);
        }

        public ViewResult Nuevo() {
            // lo tengo que crear porque necesito el ID para guardar archivos
            // inmediatamente
            var VM = new Incidente();
            VM.Tipo = TipoIncidente.Feature.Codigo;
            VM.Fecha = DateTime.Now;
            VM.US_ID = Sitio.Usuario.UsuarioID;
            DbHelper.CurrentDb().Save(VM);
            return View("Editar", VM);
        }

        public ActionResult Informe(int id) {
            var VM = Incidente.SingleOrDefault(id);
            return View(VM);
        }

        public ActionResult Editar(int id) {
            var VM = Incidente.SingleOrDefault(id);
            return View(VM);
        }

        [HttpPost]
        public ActionResult Editar(Incidente form, string actionType) {
            if (ModelState.IsValid && form.IsValid(ModelState)) {
                var db = DbHelper.CurrentDb(); bool IsNew = false;
                Incidente rec = null;
                int IncidenteID = 0;
                // nunca viene vacío porque al crearlo por primera vez ya lo crea en base de datos
                if (!form.IncidenteID.IsEmpty()) {
                    rec = Incidente.SingleOrDefault(form.IncidenteID);
                    IsNew = rec.Titulo.IsEmpty();
                    UpdateModel(rec);
                }

                using (var scope = db.GetTransaction()) {
                    //pongo algunos valores por default
                    db.SaveAndLog(rec);
                    IncidenteID = rec.IncidenteID;


                    //luego lo subscribo a quien lo editó
                    if (db.SingleOrDefault<int>("SELECT COUNT(*) from IncidenteSubscripcion where IncidenteID = @0 AND US_ID = @1", rec.IncidenteID, Sitio.Usuario.UsuarioID) == 0 ){
                        db.Execute("INSERT INTO IncidenteSubscripcion (IncidenteID, US_ID) Values (@0, @1)", rec.IncidenteID, Sitio.Usuario.UsuarioID);
                    }

                    // si es nuevo tambien subscribo al administrador
                    if (IsNew) {
                        if (!Sitio.EsDeveloper()) {
                            db.Execute("INSERT INTO IncidenteSubscripcion (IncidenteID, US_ID) Values (@0, @1)", rec.IncidenteID, Sitio.Usuario.UsuarioID);
                        }
                        Notificar(form.IncidenteID, Sitio.Usuario.Nombre+ " creó el incidente " + rec.Titulo);
                    }
                    else {
                        Notificar(form.IncidenteID, Sitio.Usuario.Nombre + " editó el incidente " + rec.Titulo);
                    }

                    scope.Complete();
                }

                TempData["InfoMessage"] = "Incidente guardado con exito";
                return Redirect("/Incidentes/ver/" + rec.IncidenteID.ToString());
            }
            return View("Editar", form);
        }

        [HttpGet]
        public ActionResult Borrar(int id) {
            var db = DbHelper.CurrentDb();
            var Incidente = db.SingleOrDefault<Incidente>(id);
            return View(Incidente);
        }


        [HttpPost]
        public ActionResult Borrar(int id, FormCollection form) {
            try {
                var db = DbHelper.CurrentDb();
                db.Execute("DELETE FROM IncidenteComentario WHERE IncidenteID = @0 DELETE FROM IncidenteSubscripcion WHERE IncidenteID = @0 DELETE FROM Incidente WHERE IncidenteID = @0", id);
                db.LogDelete(typeof(Incidente), id);

                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Incidente borrado con exito";
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
            ListarIncidentesViewModel VM = new ListarIncidentesViewModel();
            VM.q = "";
            VM.CantidadPorPagina = 0;
            VM.CalcResultado();

            dynamic ee = new BizLibMVC.ExcelExporter("Incidentes", BizLibMVC.ExcelExporter.Column.GetList(new BizLibMVC.ExcelExporter.Column("Nombre", "Nombre", BizLibMVC.ExcelExporter.ColumnType.StringCol, 100), new BizLibMVC.ExcelExporter.Column("Email", "email", BizLibMVC.ExcelExporter.ColumnType.StringCol, 100), new BizLibMVC.ExcelExporter.Column("Tipo", "AQue", BizLibMVC.ExcelExporter.ColumnType.StringCol, 50), new BizLibMVC.ExcelExporter.Column("Fecha Incidente", "DateOpened", BizLibMVC.ExcelExporter.ColumnType.DateTimeCol, 50)), VM.Resultado);
            ee.WriteToResponse(System.Web.HttpContext.Current);
            return null;
        }



        // este es el upload de imagenes de redactorJs
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadImagen(int id, HttpPostedFileBase FileData) {

            try {
                int IncidenteID = BizLibMVC.Utiles.TryCInt(id);

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
                    var Folder = Server.MapPath("/content/subidos/Incidente/");
                    if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                    file.SaveAs(Folder + FinalFileName);

                    dynamic retval = new {filelink = "/content/subidos/Incidente/" + FinalFileName};

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
                int IncidenteID = BizLibMVC.Utiles.TryCInt(id);

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
                    var Folder = Server.MapPath("/content/subidos/Incidente/");
                    if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                    file.SaveAs(Folder + FinalFileName);

                    IncidenteComentario wa = new IncidenteComentario();
                    wa.Archivo = FinalFileName;
                    wa.ArchivoNombre = sFileName;
                    wa.Fecha = DateTime.Now;
                    wa.US_ID = Sitio.Usuario.UsuarioID;
                    wa.IsDeleted = false;
                    wa.IncidenteID = IncidenteID;
                    DbHelper.CurrentDb().Save(wa);
                    Notificar(IncidenteID, Sitio.Usuario.Nombre+ " subió el archivo " + sFileName + " al incidente #" + IncidenteID.ToString());
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
                return Redirect("/Incidentes/ver/" + id);
            }

            int IncidenteID = BizLibMVC.Utiles.TryCInt(id);

            try {
                IncidenteComentario com = new IncidenteComentario();
                com.IncidenteID = IncidenteID;
                com.Mensaje = form["Mensaje"];
                com.Fecha = DateTime.Now;
                com.US_ID = Sitio.Usuario.UsuarioID;
                DbHelper.CurrentDb().Save(com);
                Notificar(IncidenteID, Sitio.Usuario.Nombre+ " comentó: &#8220;" + com.Mensaje + "&#8221;" + " en el incidente #" + IncidenteID.ToString());
                TempData["InfoMessage"] = "Comentario agregado con éxito";
                return Redirect("/Incidentes/ver/" + id + "/#comentarios");
            }
            catch (Exception ex) {
                TempData["ErrorMessage"] = "No se pudo guardar el comentario. Error: " + ex.Message;
                return Redirect("/Incidentes/ver/" + id);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BorrarComentario(string id, FormCollection form) {
            try {
                int ComentarioID = BizLibMVC.Utiles.TryCInt(id);
                dynamic Comentario = IncidenteComentario.SingleOrDefault(ComentarioID);
                if (Seguridad.CanAccess((int)Seguridad.Permisos.Modificar_Cualquier_Incidente) || Comentario.US_ID == Sitio.Usuario.UsuarioID) {
                    DbHelper.CurrentDb().Execute("UPDATE IncidenteComentario SET IsDeleted = 1 where IncidenteComentarioID = @0", ComentarioID);
                }
                else {
                    throw new ApplicationException("Solo lo puede borrar su autor");
                }
                Notificar(Comentario.IncidenteID, Sitio.Usuario.Nombre+ " borró el comentario &#8220;" + Comentario.Mensaje + "&#8221;" + " en el incidente #" + Comentario.IncidenteID.ToString());
                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Borrado con éxito";
                    return Redirect("/Incidentes/ver/" + Comentario.IncidenteID);
                }
            }
            catch (Exception ex) {
                if (Request.IsAjaxRequest()) {
                    return Content("Error: " + ex.Message);
                }
                else {
                    TempData["ErrorMessage"] = "Error: " + ex.Message;
                    return Redirect("/Incidentes/");
                }
            }
        }

        public ActionResult Cerrar(string id, FormCollection form) {
            return CambiarEstado(id, true);
        }

        public ActionResult Abrir(string id, FormCollection form) {
            return CambiarEstado(id, false);
        }

        private ActionResult CambiarEstado(string id, bool cerrado) {
            try {
                int IncidenteID = BizLibMVC.Utiles.TryCInt(id);
                Incidente Incidente = Incidente.SingleOrDefault(IncidenteID);
                if (Seguridad.CanAccess((int)Seguridad.Permisos.Modificar_Cualquier_Incidente) || Incidente.US_ID == Sitio.Usuario.UsuarioID) {
                    var db = DbHelper.CurrentDb();
                    db.BeginTransaction();
                    db.Execute("UPDATE Incidente SET Cerrado = @1 where IncidenteID = @0", IncidenteID, cerrado);
                    IncidenteComentario com = new IncidenteComentario();
                    com.IncidenteID = IncidenteID;
                    com.Status = cerrado ? "cerrado" : "abierto";
                    com.Fecha = DateTime.Now;
                    com.US_ID = Sitio.Usuario.UsuarioID;
                    db.Save(com);
                    db.CompleteTransaction();
                    Notificar(IncidenteID, Sitio.Usuario.Nombre+ " " + (cerrado ? "cerró" : "reabrió") + " el incidente #" + IncidenteID.ToString());
                }
                else {
                    throw new ApplicationException("Solo puede cambiar el estado un supervisor o el autor");
                }
                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Se cambió el estado con éxito";
                    return Redirect("/Incidentes/ver/" + Incidente.IncidenteID.ToString());
                }
            }
            catch (Exception ex) {
                if (Request.IsAjaxRequest()) {
                    return Content("Error: " + ex.Message);
                }
                else {
                    TempData["ErrorMessage"] = "Error: " + ex.Message;
                    return Redirect("/Incidentes/");
                }
            }

        }

        public ActionResult Resolvelo(string id, bool ResueltoDev) {
            try {
                int IncidenteID = BizLibMVC.Utiles.TryCInt(id);
                Incidente Incidente = Incidente.SingleOrDefault(IncidenteID);
                if (Sitio.EsDeveloper()) {
                    var db = DbHelper.CurrentDb();
                    db.Execute("UPDATE Incidente SET ResueltoDev = @1 where IncidenteID = @0", IncidenteID, ResueltoDev);
                }
                else {
                    throw new ApplicationException("No permitido");
                }
                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Se cambió con éxito";
                    return Redirect("/Incidentes/");
                }
            }
            catch (Exception ex) {
                if (Request.IsAjaxRequest()) {
                    return Content("Error: " + ex.Message);
                }
                else {
                    TempData["ErrorMessage"] = "Error: " + ex.Message;
                    return Redirect("/Incidentes/");
                }
            }

        }


        public ActionResult AgregarUsuario(int id) {
            return PartialView(id);
        }

        [HttpPost]
        public ActionResult AgregarUsuario(int IncidenteID, int US_ID) {
            var db = DbHelper.CurrentDb();
            var rec = db.SingleOrDefault<dynamic>("SELECT * FROM IncidenteSubscripcion WHERE IncidenteID = @0 AND US_ID = @1", IncidenteID, US_ID);
            if (rec == null) {
                db.Execute("INSERT INTO IncidenteSubscripcion ( IncidenteID, US_ID ) VALUES (@0,@1)", IncidenteID, US_ID);
                var Nombre = Models.Usuario.SingleOrDefault(US_ID).Nombre;
                Notificar(IncidenteID, Nombre + " fue agregado a las notificaciones del incidente por " + Sitio.Usuario.Nombre);
            }
            return Content("OK");
        }


        public ActionResult EstadoSubscripcion(int id) {
            var vm = new EstadoSubscripcionViewModel();
            vm.IncidenteID = id;
            vm.EstaSubscripto = DbHelper.CurrentDb().ExecuteScalar<int>("SELECT COUNT(*) FROM IncidenteSubscripcion Where IncidenteID = @0 and US_ID = @1", id, Sitio.Usuario.UsuarioID) > 0;
            return PartialView(vm);
        }

        [HttpPost()]
        public ActionResult Subscribir(int id) {
            try {
                DbHelper.CurrentDb().Execute("INSERT INTO IncidenteSubscripcion ( IncidenteID, US_ID ) VALUES (@0,@1)", id, Sitio.Usuario.UsuarioID);
                return Content("OK");
            }
            catch (Exception ex) {
                return Content("Error:" + ex.Message);
            }
        }


        [HttpPost()]
        public ActionResult Desuscribir(int id) {
            try {
                DbHelper.CurrentDb().Execute("DELETE FROM IncidenteSubscripcion WHERE IncidenteID = @0 AND US_ID = @1", id, Sitio.Usuario.UsuarioID);
                return Content("OK");
            }
            catch (Exception ex) {
                return Content("Error:" + ex.Message);
            }
        }

        private void Notificar(int IncidenteID, string Mensaje) {
            try {
                //si el metodo falla, no quiero que pase nada, porque es notificativo
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                var db = DbHelper.CurrentDb();
                string sqlstmt = @"select Usuario.*  
                                from Usuario  
                                inner join IncidenteSubscripcion on Usuario.UsuarioID = IncidenteSubscripcion.US_ID
                                where Usuario.UsuarioID <> @0
                                    and IncidenteSubscripcion.IncidenteID = @1";
                var lista = db.Fetch<Usuario>(sqlstmt, Sitio.Usuario.UsuarioID, IncidenteID);

                foreach (Usuario u in lista) {
                    Notificacion.Notificar(u.UsuarioID, Mensaje, "/incidentes/ver/" + IncidenteID.ToString());
                }

            }
            catch (Exception) {
            }
        }


    }
}
namespace Site.Models {
    public class ListarIncidentesViewModel {
        
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public bool ExcluirCerrados { get; set; }
        public bool SoloSubscriptos { get; set; }
        public bool SoloSinResolver { get; set; }
        public string Tipo { get; set; }

        public List<Incidente> Resultado { get; set; }

        public ListarIncidentesViewModel() {
            q = Sitio.GetPref("ListarIncidentes-q", "");
            CantidadPorPagina = Sitio.GetPref("ListarIncidentes-CantidadPorPagina", 50);
            ExcluirCerrados = Sitio.GetPref("ListarIncidentes-ExcluirCerrados", true);
            SoloSubscriptos = Sitio.GetPref("ListarIncidentes-SoloSubscriptos", false);
            SoloSinResolver = Sitio.GetPref("ListarIncidentes-SoloSinResolver", Sitio.EsDeveloper());
            Tipo = Sitio.GetPref("ListarIncidentes-Tipo", "");
        }

        public void SetPref() {
            Sitio.SetPref("ListarIncidentes-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("ListarIncidentes-q", q);
            Sitio.SetPref("ListarIncidentes-ExcluirCerrados", ExcluirCerrados);
            Sitio.SetPref("ListarIncidentes-SoloSubscriptos", SoloSubscriptos);
            Sitio.SetPref("ListarIncidentes-SoloSinResolver", SoloSinResolver);
            Sitio.SetPref("ListarIncidentes-Tipo", Tipo);
        }

        public void CalcResultado() {
            var sql = Incidente.BaseQuery(this.CantidadPorPagina);
            sql.Append("WHERE Incidente.Titulo IS NOT NULL"); // por las dudas que haya cosas en blanco


            if (!this.Tipo.IsEmpty()) {
                sql.Append("AND Incidente.Tipo = @0", this.Tipo);
            }


            if (this.ExcluirCerrados) {
                sql.Append("AND Incidente.Cerrado = 0");
            }

            if (this.SoloSubscriptos) {
                sql.Append("AND (Incidente.US_ID = @0 OR Exists (SELECT * from IncidenteSubscripcion Where IncidenteSubscripcion.US_ID = @0 and IncidenteSubscripcion.IncidenteID = Incidente.IncidenteID))", Sitio.Usuario.UsuarioID);
            }

            if (SoloSinResolver) {
                sql.Append("AND Incidente.ResueltoDev = 0");
            }
            if (!Sitio.EsEmpleado)
            {
                if (Sitio.Usuario.ProveedorID.IsEmpty())
                    sql.Append("AND Incidente.US_ID = @0", Sitio.Usuario.UsuarioID);
                else
                    sql.Append("AND (Incidente.US_ID = @0 OR Incidente.US_ID = @1)", Sitio.Usuario.UsuarioID, Sitio.Usuario.ProveedorID);
            }

            if (!this.q.IsEmpty())
                sql.AppendKeywordMatching(this.q, "Incidente.Titulo", "Incidente.Notas");

            sql.Append("ORDER BY Incidente.Fecha");
            Resultado = DbHelper.CurrentDb().Fetch<Incidente>(sql);
        }
    }

    public class EstadoSubscripcionViewModel {
        public bool EstaSubscripto { get; set; }
        public int IncidenteID { get; set; }
    }

    public class InformeIncidentesCerradosViewModel {
        public int CantidadPorPagina { get; set; }
        public DateTime FechaDesde { get; set; }
        [FutureDateTime]
        public DateTime FechaHasta { get; set; }

        public List<dynamic> Resultado { get; set; }

        public InformeIncidentesCerradosViewModel() {
            CantidadPorPagina = Sitio.GetPref("InformeIncidentesCerrados-CantidadPorPagina", "50").AsInt();
            FechaDesde = Sitio.GetPref("InformeIncidentesCerrados-FechaDesde", DateTime.Today.AddMonths(-1).FirstDayOfMonth().ToISO()).ISOToDate();
            FechaHasta = Sitio.GetPref("InformeIncidentesCerrados-FechaHasta", DateTime.Today.AddMonths(-1).LastDayOfMonth().ToISO()).ISOToDate();

        }

        public void SetPref() {
            Sitio.SetPref("InformeIncidentesCerrados-CantidadPorPagina", CantidadPorPagina.ToString());
            Sitio.SetPref("InformeIncidentesCerrados-FechaDesde", FechaDesde.ToISO());
            Sitio.SetPref("InformeIncidentesCerrados-FechaHasta", FechaHasta.ToISO());

        }

        public void CalcResultado() {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(CantidadPorPagina);
            sql.Append(@"
                Incidente.IncidenteID, Titulo, Max(incidenteComentario.Fecha) as FechaCerrado, Substring(Notas, 1, 500) as Notas
                from incidente Inner join incidenteComentario on incidenteComentario.IncidenteID = Incidente.IncidenteID
                and incidenteComentario.status = 'cerrado'");
            sql.Append("WHERE Incidente.Titulo IS NOT NULL"); // por las dudas que haya cosas en blanco
            sql.Append("AND Incidente.Cerrado <> 0");
            if (!Sitio.EsEmpleado){
                if (Sitio.Usuario.ProveedorID.IsEmpty())
                    sql.Append("AND Incidente.US_ID = @0", Sitio.Usuario.UsuarioID);
                else
                    sql.Append("AND (Incidente.US_ID = @0 OR Incidente.US_ID = @1)", Sitio.Usuario.UsuarioID, Sitio.Usuario.ProveedorID);
            }
            sql.Append("group by Incidente.IncidenteID, Titulo, Substring(Notas, 1, 500)");
            sql.Append("having Max(incidenteComentario.Fecha) between @0 and @1", FechaDesde, FechaHasta);
            sql.Append("ORDER BY FechaCerrado");
            Resultado = DbHelper.CurrentDb().Fetch<dynamic>(sql);
        }

        public System.Web.Helpers.BizGridColumn[] Columns() {
            var Grilla = new System.Web.Helpers.BizGrid();
            return Grilla.Columns(
                    Grilla.Column("IncidenteID", "Nro", format: item => new System.Web.WebPages.HelperResult(writer => {
                        HelperPage.WriteLiteralTo(writer, string.Format("<a href='/incidentes/ver/{0}'>{0}</a>", item.IncidenteID));
                    })),

                    Grilla.Column("Incidente", "Incidente", format: item => new System.Web.WebPages.HelperResult(writer => {
                            HelperPage.WriteLiteralTo(writer, string.Format("<b>{0}</b>", item.Titulo));
                            HelperPage.WriteLiteralTo(writer, string.Format("<br>{0}", item.Notas ?? ""));
                            HelperPage.WriteLiteralTo(writer, "</div>");
                    })),
                    Grilla.Column("FechaCerrado", "Fecha Cerrado")
            );
        }
    }
}
