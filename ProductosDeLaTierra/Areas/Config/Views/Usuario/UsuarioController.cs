using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;
using System.Web.WebPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DataAnnotationsExtensions;

namespace Site.Areas.Config.Controllers {

    [Authorize]
    public class UsuarioController : ApplicationController {
        const Seguridad.Permisos Permiso = Seguridad.Permisos.Base_de_datos_Seguridad;

        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Index()
        {
            var VM = new ListarUsuariosViewModel();
            VM.CalcResultado();
            return View(VM);
        }

        [HttpPost]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Index(ListarUsuariosViewModel form) {

            try {
                var VM = new ListarUsuariosViewModel();
                UpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex) {
                ModelState.AddModelError("all", ex.Message);
                return View(form);
            }

        }

        [CustomAuthorize(Roles = Permiso)]
        public ViewResult Nuevo() {
            var Usuario = new Usuario();
            Usuario.Activo = true;
            Usuario.InitItems();
            return View("Editar", Usuario);
        }

        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Editar(int id)
        {
            var db = DbHelper.CurrentDb();
            var VM = Usuario.SingleOrDefault(id);
            VM.InitItems();
            return View(VM);
        }

        [CustomAuthorize(Roles = Permiso)]
        [HttpPost]
        public ActionResult Editar(Usuario form) {
            if (ModelState.IsValid && form.IsValid(ModelState)) {
                var db = DbHelper.CurrentDb();

                Usuario rec; bool IsNew = false;
                if (form.UsuarioID != 0) {
                    rec = Usuario.SingleOrDefault(form.UsuarioID);
                    UpdateModel(rec);
                }
                else {
                    rec = new Usuario();
                    UpdateModel(rec);
                    rec.Avatar = "nn.gif";
                    rec.AvatarChico = "nn50x50.gif";
                    IsNew = true;
                };

                using (var scope = db.GetTransaction()) {
                    rec.ProveedorID = rec.ProveedorID.ZeroToNull();
                    db.UpsertAndLog(rec, IsNew);

                    foreach (var item in rec.Items) {
                        if (!item.RolID.IsEmpty()) {
                            var irec = new UsuarioRol();
                            irec.UsuarioRolID = item.UsuarioRolID;
                            irec.UsuarioID = rec.UsuarioID;
                            irec.RolID = item.RolID;
                            db.Save(irec);

                        }
                        else if (!item.UsuarioRolID.IsEmpty()) {
                            // antes tenia algo y lo limpio
                            db.Execute("DELETE FROM UsuarioRol WHERE UsuarioRolID = @0", item.UsuarioRolID);
                        }
                    }


                    Seguridad.CleanAllCache();

                    scope.Complete();

                }

                if (IsNew) {
                    TempData["InfoMessage"] = "Usuario guardado con exito";
                    return RedirectToAction("EnviarPassword", new { id = rec.UsuarioID });
                }
                else {
                    TempData["InfoMessage"] = "Usuario actualizado con exito";
                    return RedirectToAction("Index");

                }


            }
            return View("Editar", form);
        }

        [HttpGet]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult EnviarPassword(int id)
        {
            var db = DbHelper.CurrentDb();
            var VM = Usuario.SingleOrDefault(id);
            return View(VM);
        }


        [HttpPost]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult EnviarPassword(int id, FormCollection form)
        {
            var db = DbHelper.CurrentDb();
            var Usuario = Models.Usuario.SingleOrDefault(id);
            if (Usuario != null)
            {
                bool EsNuevo = (Usuario.Password == null);
                string newpassword = Utils.randomLetters(4) + Utils.randomNumber(1000, 9999).ToString();
                db.Update<Usuario>("SET Password=@0 WHERE UsuarioID=@1", Usuario.GetHash(newpassword), Usuario.UsuarioID);
                Usuario.Password = newpassword;

                var email = new System.Net.Mail.MailMessage();
                email.To.Add(Usuario.Email);
                email.Subject = String.Format("Sus datos de acceso a {0}", Sitio.WebsiteURL);
                email.Body = this.RenderViewToString((EsNuevo ? "EmailNuevoUsuario" : "EmailResetPassword"), Usuario);
                email.IsBodyHtml = true;
                email.Send();

                TempData["InfoMessage"] = "Contraseña enviada con exito";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo encontrar el usuario";
                return RedirectToAction("Index");
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public object CambiarAvatar(int id, HttpPostedFileBase file1) {

            try {
                int UsuarioID = id;

                //subimos las fotos
                var FileData = Request.Files[0];
                if (FileData.ContentLength > 0) {
                    string sFileName = FileData.FileName;
                    //chequeo el tipo
                    string sFileType = System.IO.Path.GetExtension(sFileName).ToLower().Trim();
                    var permitidos = ".jpg ;.jpeg ;.gif ;.png ;";
                    if (string.IsNullOrEmpty(sFileType) || !permitidos.Contains(sFileType)) {
                        throw new ApplicationException("No se pudo subir archivo " + sFileName + ". Solo se permiten archivos con extensión " + permitidos);
                    }
                    var FinalFileName = Utils.GetRandomPasswordUsingGUID(32) + sFileType;
                    var Folder = Server.MapPath(Usuario.AvatarFolder);
                    if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                    FileData.SaveAs(Folder + FinalFileName);
                    string Avatar = Utils.SaveAsThumbnailCropped(Folder, FinalFileName, 200, 200);
                    string AvatarChico = Utils.SaveAsThumbnailCropped(Folder, FinalFileName, 50, 50);
                    var db = DbHelper.CurrentDb();
                    db.Update<Usuario>("SET Avatar=@0, AvatarChico=@1 WHERE UsuarioID=@2", Avatar, AvatarChico, UsuarioID);
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



        [HttpGet]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Borrar(int id)
        {
            var db = DbHelper.CurrentDb();
            var VM = Usuario.SingleOrDefault(id);
            return View(VM);
        }


        [HttpPost]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Borrar(int id, FormCollection form)
        {
            try {
                var db = DbHelper.CurrentDb();
                //db.Execute("DELETE FROM UsuarioContribuyente WHERE UsuarioID = @0 DELETE FROM Usuario WHERE UsuarioID = @0", id);
                
                if (Request.IsAjaxRequest())
                {
                    return Content("OK");
                }
                else
                {
                    TempData["InfoMessage"] = "Usuario borrado con exito";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return Content("Error: " + DbHelper.EnCastellanoPorFavor(ex));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error: " + DbHelper.EnCastellanoPorFavor(ex);
                    return View();
                }

            }
        }




        
        [NoCache]
        public JsonResult Lista(string term, string Rol) {
            var db = DbHelper.CurrentDb();
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT TOP 50 Usuario.UsuarioID as ID, Nombre, Usuario.Username + ' - ' + Usuario.Email as ExtraInfo");
            sql.Append("FROM Usuario");
            if (!Rol.IsEmpty()) {
                sql.Append("INNER JOIN UsuarioRol usuarioRol ON usuarioRol.UsuarioID = Usuario.UsuarioID ");
                sql.Append("WHERE usuarioRol.RolID IN (Select RolID FROM Rol Where Nombre = @0 )",Rol);
            }
            else {
                sql.Append("WHERE 1=1");
            }
            sql.AppendKeywordMatching(term, "Username", "Nombre", "Email");
            sql.Append("Order by Nombre");
            var retval = db.Query<IDNombrePar>(sql);
            return Json(retval, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult ListaValidar(string id, string Rol) {
            if (id.IsEmpty()) {
                return Json(new List<IDNombrePar>(), JsonRequestBehavior.AllowGet);
            }
            else {
                var db = DbHelper.CurrentDb();
                var sql = PetaPoco.Sql.Builder;
                sql.Append("SELECT TOP 1 Usuario.UsuarioID as ID, Usuario.Nombre as Nombre FROM Usuario");
                if (!Rol.IsEmpty()) {
                    sql.Append("INNER JOIN UsuarioRol usuarioRol ON usuarioRol.UsuarioID = Usuario.UsuarioID ");
                    sql.Append("WHERE usuarioRol.RolID IN (Select RolID FROM Rol Where Nombre = @0 )",Rol);
                }
                else {
                    sql.Append("WHERE 1=1");
                }
                sql.Append(" AND Nombre = @0 or Username = @0", id);
                var retval = db.Query<IDNombrePar>(sql);
                return Json(retval, JsonRequestBehavior.AllowGet);
            }
        } 




    }
}

namespace Site.Models {
    public class ListarUsuariosViewModel {
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public bool SoloActivos { get; set; }

        public List<Usuario> Resultado { get; set; }

        public ListarUsuariosViewModel() {
            CantidadPorPagina = Sitio.GetPref("ListarUsuarios-CantidadPorPagina", 50);
            SoloActivos = Sitio.GetPref("ListarUsuarios-SoloActivos", true);
            q = Sitio.GetPref("ListarUsuarios-q", "");
            Resultado = new List<Usuario>();
        }

        public void SetPref()
        {
            Sitio.SetPref("ListarUsuarios-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("ListarUsuarios-SoloActivos", SoloActivos);
            Sitio.SetPref("ListarUsuarios-q", q);
        }

        public void CalcResultado()
        {
            var sql = Models.Usuario.BaseQuery(CantidadPorPagina);
            sql.Append("WHERE 1=1");

            if (!this.q.IsEmpty())
                sql.AppendKeywordMatching(this.q, "Username", "Usuario.Nombre", "Usuario.Email");

            if (this.SoloActivos)
                sql.Append("AND Usuario.Activo <> 0");

            sql.Append("ORDER BY Usuario.Nombre");
            Resultado = DbHelper.CurrentDb().Fetch<Usuario>(sql);
        }
    }
}
