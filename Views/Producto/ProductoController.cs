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
    public class ProductoController : Controller
    {
        const Seguridad.Permisos Permiso = Seguridad.Permisos.Producto;
        //
        // GET: /Productos/
		[CustomAuthorize(Roles = Permiso)]
        public ActionResult Index(IndexProductoViewModel form) {
            try {
                var VM = new IndexProductoViewModel();
                TryUpdateModel(VM);
                VM.CalcResultado();
                VM.SetPref(); // guardo las preferencias para la próxima
                return View(VM);
            }
            catch (Exception ex) {
                ModelState.AddModelError("all", ex.Message);
                form.Resultado = new List<Models.Producto>();
                return View(form);
            }
	    }

        [CustomAuthorize(Roles=Permiso)]
        public ActionResult Nuevo() {
            Producto VM = new Producto();
            // si es empleado de la empresa tendra que elegir el usuario, sino el usuario es el actual.
            VM.ProveedorID = Sitio.EsEmpleado ? VM.ProveedorID : Sitio.Usuario.ProveedorID.IsEmpty()?Sitio.Usuario.UsuarioID:Sitio.Usuario.ProveedorID??0;
            if (Request.IsAjaxRequest())
                return PartialView("Editar", VM);
            else
                return View("Editar", VM);
            
        }

                
		[CustomAuthorize(Roles = Permiso)]
        public ActionResult Editar(int id) {
            Producto VM = Producto.SingleOrDefault(id);
            if (Request.IsAjaxRequest())
                return PartialView("Editar", VM);
            else
                return View("Editar", VM);
        }

        [HttpPost]
		[CustomAuthorizeEditar(Roles = Permiso)]
        public ActionResult Editar(Producto form, string actionType) {
            if (ModelState.IsValid && form.IsValid(ModelState) /*&& ValidateProducto(form,ModelState)*/) {
                var db = DbHelper.CurrentDb();
                Producto rec;
                if (form.ProductoID != 0) {
                    rec = Producto.SingleOrDefault(form.ProductoID);                    
                }
                else {
                    rec = new Producto();
                };
                TryUpdateModel(rec);

                using (var scope = db.GetTransaction()) {
                    db.SaveAndLog(rec);
                    scope.Complete();
                }

                if (Request.IsAjaxRequest()) {
                    return Content("OK");
                }
                else {
                    TempData["InfoMessage"] = "Registro guardado con éxito";
                    return RedirectToAction((actionType == "Guardar" ? "Editar" : (actionType == "Guardar y nuevo" ? "Nuevo": "Index")), new { id = rec.ProductoID });
                }
            }
            if (Request.IsAjaxRequest())
                return Content("Error " + ModelState.ToHTMLString());
            else
                return View("Editar", form);
        }

        private bool ValidateProducto(Producto Producto, ModelStateDictionary ModelState){                                   
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT * FROM ItemMercaderia WHERE ProductoID = @0 ", Producto.ProductoID);
            var otroEventosID = DbHelper.CurrentDb().Fetch<int>(sql);
            if (otroEventosID.Count > 0) {
                ModelState.AddModelError("","No puede modificarse el Producto porque participa de otros eventos");
            }
            else {
                return true;
            }
            return false;
        }
    
        [HttpGet]
		[CustomAuthorizeBorrar(Roles = Permiso)]
        public ActionResult Borrar(int id) {
            var db = DbHelper.CurrentDb();
            var Producto = db.SingleOrDefault<Producto>(id);
            return View(Producto);
        }


        [HttpPost]
		[CustomAuthorizeBorrar(Roles = Permiso)]
        public ActionResult Borrar(int id, FormCollection form) {
            try {
                Producto Producto = Producto.SingleOrDefault(id);
                if (Producto.CurrentUserCanAccessToFunction( Seguridad.Feature.Borrar)) {
                    Producto.Delete();
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
            var Producto = Models.Producto.SingleOrDefault(id);
            return PartialView(Producto);
        }
        
		[CustomAuthorize(Roles = Permiso)]
        public ActionResult ExtraInfo(int id) {
            var VM = Models.Producto.SingleOrDefault(id);
            return PartialView("MasInfo", VM);
        }
        

        [NoCache]
        public JsonResult Lista(string term, int? ProveedorID) {
            var retval = new List<InfoProducto>();
            var db = DbHelper.CurrentDb();
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT TOP 25 ProductoID as ID, ISNULL(Convert(varchar(12),Producto.CodigoArticulo ),'') + ' - ' + ISNULL(Convert(varchar(50),Descripcion),'') as Nombre,  'Proveedor: '+ usuario.Nombre as ExtraInfo, Producto.PesoUnitario as PesoUnitario, Producto.PrecioUnitario as PrecioUnitario, Producto.PrecioKg as PrecioKg, Producto.UsuarioID as ProveedorID");
            sql.Append("FROM Producto");
            sql.Append("INNER JOIN Usuario usuario ON usuario.UsuarioID = Producto.UsuarioID");
            sql.Append("Where 1=1");
            if (!ProveedorID.IsEmpty()) {
                sql.Append("AND Producto.UsuarioID = @0",ProveedorID??0);
            }
            sql.AppendKeywordMatching(term, "Producto.CodigoArticulo", "Producto.Descripcion");
            sql.Append("ORDER BY Producto.CodigoArticulo");
            retval = db.Query<InfoProducto>(sql).ToList();
            return Json(retval, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult ListaValidar(string id, int ProveedorID) {
            var retval = new List<InfoProducto>();
            if (!id.IsEmpty()) {
                var db = DbHelper.CurrentDb();
                var sql = PetaPoco.Sql.Builder;
                sql.Append("SELECT TOP 1 ProductoID as ID, ISNULL(Convert(varchar(12),Producto.CodigoArticulo ),'') + ' - ' + ISNULL(Convert(varchar(40),Descripcion) as Nombre, Producto.PesoUnitario as PesoUnitario, Producto.PrecioUnitario as PrecioUnitario, Producto.PrecioKg as PrecioKg, Producto.UsuarioID as ProveedorID");
                sql.Append("FROM Producto");
                sql.Append("Where (Producto.CodigoArticulo = @0 OR Producto.Descripcion=@0 )", id);
                if (!ProveedorID.IsEmpty()) {
                    sql.Append("AND Producto.UsuarioID = @0",ProveedorID);
                }

                retval = db.Query<InfoProducto>(sql).ToList();
                return Json(retval, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(retval, JsonRequestBehavior.AllowGet);
            }
            
        }

        public class InfoProducto {
            public int ID { get; set; }
            public string Nombre { get; set; }
            public string ExtraInfo { get; set; }
            public double PrecioKg { get; set; }
            public double PesoUnitario { get; set; }
            public double PrecioUnitario { get; set; }
            public int ProveedorID { get; set; }
            public InfoProducto() { }
            public InfoProducto(int ID, string Nombre,double PrecioKg,double PrecioUnitario,int ProveedorID) {
                this.ID = ID;
                this.Nombre = Nombre;
                this.PrecioKg = PrecioKg;
                this.PrecioUnitario = PrecioUnitario;
                this.ProveedorID = ProveedorID;
            }
        }
    }

}
