using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;
using System.Web.WebPages;

namespace Site.Areas.Config.Controllers {
    public class PermisosController : ApplicationController
    {
        const Seguridad.Permisos ModuloPermiso = Seguridad.Permisos.Base_de_datos_Seguridad;

        [CustomAuthorize(Roles = ModuloPermiso)]
        public ActionResult Index()
        {
            var VM = new ListarPermisosViewModel();
            VM.CalcResultado();
            return View(VM);
        }

        [HttpPost]
        [CustomAuthorize(Roles = ModuloPermiso)]
        public ActionResult Index(ListarPermisosViewModel form)
        {

            try
            {
                var VM = new ListarPermisosViewModel();
                UpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("all", ex.Message);
                return View(form);
            }

        }

        [CustomAuthorize(Roles = ModuloPermiso)]
        public ActionResult Editar(int id)
        {
            var db = DbHelper.CurrentDb();
            var Permiso = db.SingleOrDefault<Permiso>(id);
            return PartialView(Permiso);
        }

        [CustomAuthorize(Roles = ModuloPermiso)]
        public ActionResult EditarRoles(int id) {
            var db = DbHelper.CurrentDb();
            var Permiso = db.SingleOrDefault<Permiso>(id);
            Permiso.InitRolesConPermiso();
            return PartialView(Permiso);
        }

        [CustomAuthorize(Roles = ModuloPermiso)]
        public ActionResult EditarUsuarios(int id) {
            var db = DbHelper.CurrentDb();
            var Permiso = db.SingleOrDefault<Permiso>(id);
            Permiso.InitUsuariosConPermiso();
            return PartialView(Permiso);
        }


        // Esta edición solo edita los 
        [HttpPost]
        [CustomAuthorize(Roles = ModuloPermiso)]
        public ActionResult Editar(Permiso form) {
            try {
                var db = DbHelper.CurrentDb();
                int PermisoID = 0;
                var rec = Permiso.SingleOrDefault(form.PermisoID);
                rec.RolesConPermiso = new List<PermisoConcedido>();
                rec.UsuariosConPermiso = new List<PermisoConcedido>();
                UpdateModel(rec);
                using (var scope = db.GetTransaction()) {
                    PermisoID = rec.PermisoID;

                    // guardo el registro solo si me mando los datos, sino, significa que esta
                    // actualizando los permisos
                    if (!rec.Nombre.IsEmpty()) db.Save(rec);

                    Seguridad.CleanAllCache();

                    foreach (PermisoConcedido item in rec.RolesConPermiso) {
                        if (!item.RolID.IsEmpty() && (item.PuedeEntrar || item.PuedeEditar || item.PuedeBorrar)) {
                            var irec = new PermisoConcedido();
                            irec.PermisoID = PermisoID;
                            irec.RolID = item.RolID;
                            irec.PermisoConcedidoID = item.PermisoConcedidoID;
                            irec.PuedeBorrar = item.PuedeBorrar;
                            irec.PuedeEditar = item.PuedeEditar;
                            irec.PuedeEntrar = item.PuedeEntrar;
                            db.Save(irec);
                        }
                        else if (!item.PermisoConcedidoID.IsEmpty()) {
                            // antes tenia algo y lo limpio
                            db.Execute("DELETE FROM PermisoConcedido WHERE PermisoConcedidoID = @0", item.PermisoConcedidoID);
                        }
                    }

                    // ahora los usuarios
                    foreach (PermisoConcedido item in rec.UsuariosConPermiso) {
                        // ya estoy limpiando todo el cache, asi que no importa esto
                        //if (!item.UsuarioID.IsEmpty()) {
                        //    Seguridad.CleanCache((int)item.UsuarioID);
                        //}

                        if (!item.UsuarioID.IsEmpty() && (item.PuedeEntrar || item.PuedeEditar || item.PuedeBorrar)) {
                            var irec = new PermisoConcedido();
                            irec.PermisoID = PermisoID;
                            irec.UsuarioID = item.UsuarioID;
                            irec.PermisoConcedidoID = item.PermisoConcedidoID;
                            irec.PuedeBorrar = item.PuedeBorrar;
                            irec.PuedeEditar = item.PuedeEditar;
                            irec.PuedeEntrar = item.PuedeEntrar;
                            db.Save(irec);
                        }
                        else if (!item.PermisoConcedidoID.IsEmpty()) {
                            // antes tenia algo y lo limpio
                            db.Execute("DELETE FROM PermisoConcedido WHERE PermisoConcedidoID = @0", item.PermisoConcedidoID);
                        }
                    }

                    scope.Complete();
                }

                return Content("OK");
            }
            catch (Exception ex) {
                return Content("Error: " + ex.Message);
            }
        }
        public ContentResult CantRolesDescrip(int id) {
            var o = Permiso.SingleOrDefault(id);
            return Content(o.RolesConPermiso.Count().ToString() + " roles");
        }
        public ContentResult CantUsuariosDescrip(int id) {
            var o = Permiso.SingleOrDefault(id);
            return Content(o.UsuariosConPermiso.Count().ToString() + " usuarios");
        }
    }
}

namespace Site.Models
{
    public class ListarPermisosViewModel
    {
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public List<Permiso> Resultado { get; set; }

        public ListarPermisosViewModel() {
            CantidadPorPagina = Sitio.GetPref("ListarPermisos-CantidadPorPagina", 50);
            q = Sitio.GetPref("ListarPermisos-q", "");
            Resultado = new List<Permiso>();
        }

        public void SetPref() {
            Sitio.SetPref("ListarPermisos-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("ListarPermisos-q", q);
        }

        public void CalcResultado()
        {
            var sql = PetaPoco.Sql.Builder;
            sql.AppendSelectTop(this.CantidadPorPagina);
            sql.Append("Permiso.*, ");
            sql.Append("(select count(*) from PermisoConcedido where permiso.PermisoID = PermisoConcedido.PermisoID and RolID is not null and (PuedeEntrar <> 0 or PuedeEditar <> 0 or PuedeBorrar <> 0)) as CantRoles,");
            sql.Append("(select count(*) from PermisoConcedido where permiso.PermisoID = PermisoConcedido.PermisoID and UsuarioID is not null and (PuedeEntrar <> 0 or PuedeEditar <> 0 or PuedeBorrar <> 0)) as CantUsuarios");
            sql.Append("FROM Permiso");
            sql.Append("WHERE Permiso.Activo <> 0");

            if (!this.q.IsEmpty())
                sql.AppendKeywordMatching(this.q, "Permiso.Nombre", "Permiso.Notas");

            sql.Append("ORDER BY Permiso.Nombre");
            Resultado = DbHelper.CurrentDb().Fetch<Permiso>(sql);
        }
    }
}
