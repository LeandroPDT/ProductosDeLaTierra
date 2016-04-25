using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;
using System.Web.WebPages;

namespace Site.Areas.Config.Controllers {
    [Authorize]
    public class RolesController : ApplicationController {

        const Seguridad.Permisos Permiso = Seguridad.Permisos.Base_de_datos_Seguridad;

        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Index() {
            var VM = new ListarRolesViewModel();
            VM.CalcResultado();
            return View(VM);
        }

        [HttpPost]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Index(ListarRolesViewModel form) {

            try {
                var VM = new ListarRolesViewModel();
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
            var Rol = new Rol();
            Rol.InitItems();
            return View("Editar", Rol);
        }

        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Editar(int id) {
            var VM = Rol.SingleOrDefault(id);
            VM.InitItems();
            return View(VM);
        }

        [HttpPost]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Editar(Rol form) {
            if (ModelState.IsValid && form.IsValid(ModelState)) {
                var db = DbHelper.CurrentDb();
                Rol rec; bool IsNew = false;
                if (form.RolID != 0) {
                    rec = Rol.SingleOrDefault(form.RolID);
                    UpdateModel(rec);
                }
                else {
                    rec = new Rol();
                    UpdateModel(rec);
                    IsNew = true;
                };

                using (var scope = db.GetTransaction()) {
                    db.UpsertAndLog(rec, IsNew);

                    db.Execute("DELETE FROM UsuarioRol WHERE RolID = @0", rec.RolID);
                    foreach (IDNombrePar item in rec.Items) {
                        if (!item.ID.IsEmpty()) {
                            db.Execute("Insert into UsuarioRol (UsuarioID, RolID) values (@0, @1)", item.ID, rec.RolID);
                        }
                    }
                    scope.Complete();
                }

                TempData["InfoMessage"] = "Rol guardado con exito";
                return RedirectToAction("Index");

            }
            return View("Editar", form);
        }


        [HttpGet]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Borrar(int id)
        {
            var db = DbHelper.CurrentDb();
            var Rol = db.SingleOrDefault<Rol>(id);
            return View(Rol);
        }


        [HttpPost]
        [CustomAuthorize(Roles = Permiso)]
        public ActionResult Borrar(int id, FormCollection form) {
            try {
                var db = DbHelper.CurrentDb();
                db.Execute("DELETE FROM UsuarioRol WHERE RolID = @0 DELETE FROM PermisoConcedido WHERE RolID = @0 DELETE FROM Rol WHERE RolID = @0", id);
                db.LogDelete(typeof(Rol), id);
                
                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Rol borrado con exito";
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

        [NoCache]
        public JsonResult Lista(string term) {
            var db = DbHelper.CurrentDb();
            var retval = db.Query<IDNombrePar>("SELECT TOP 25 RolID as ID, Nombre FROM Rol where Nombre like '%" + term + "%' ORDER By Nombre");
            return Json(retval, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult ListaValidar(string id) {
            if (id.IsEmpty()) {
                return Json(new List<IDNombrePar>(), JsonRequestBehavior.AllowGet);
            }
            else {
                var db = DbHelper.CurrentDb();
                var retval = db.Query<IDNombrePar>("SELECT TOP 1 RolID as ID, Nombre FROM Rol where Nombre = @0", id);
                return Json(retval, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Permisos(int id) {
            var db = DbHelper.CurrentDb();
            var sql = PetaPoco.Sql.Builder;
            sql.Append("Select Permiso.Nombre, Permiso.Notas, PuedeEntrar, PuedeEditar, PuedeBorrar");
            sql.Append("FROM Permiso");
            sql.Append("   Inner Join PermisoConcedido on PermisoConcedido.PermisoID = Permiso.PermisoID");
            sql.Append("WHERE PermisoConcedido.RolID =@0", id);
            sql.Append("ORDER BY Permiso.Nombre");

            var Grilla = new System.Web.Helpers.BizGrid(db.Fetch<dynamic>(sql));
            Grilla.Filtrable = false;
            System.Web.Helpers.BizGridColumn[] Columnas = Grilla.Columns(
                Grilla.Column("Nombre", "Nombre"),
                Grilla.Column("Notas"),
                Grilla.Column("PuedeEntrar", "Puede Acceder"),
                Grilla.Column("PuedeEditar", "Puede Editar"),
                Grilla.Column("PuedeBorrar", "Puede Borrar")
            );

            return Content(Grilla.GetHtml(columns: Columnas).ToString());

        }


    }
}

namespace Site.Models {
    public class ListarRolesViewModel {
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public List<Rol> Resultado { get; set; }

        public ListarRolesViewModel() {
            CantidadPorPagina = Sitio.GetPref("ListarRoles-CantidadPorPagina", 50);
            q = Sitio.GetPref("ListarRoles-q", "");
            Resultado = new List<Rol>();
        }

        public void SetPref() {
            Sitio.SetPref("ListarRoles-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("ListarRoles-q", q);
        }

        public void CalcResultado() {
            var sql = PetaPoco.Sql.Builder;
            sql.AppendSelectTop(this.CantidadPorPagina);
            sql.Append("*");
            sql.Append("FROM Rol");
            sql.Append("WHERE 1=1");


            if (!this.q.IsEmpty()) {
                sql.AppendKeywordMatching(this.q, "Nombre");
            }

            sql.Append("ORDER BY Nombre");
            Resultado = DbHelper.CurrentDb().Fetch<Rol>(sql);
        }
    }
}
