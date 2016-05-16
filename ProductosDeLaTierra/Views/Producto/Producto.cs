using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;
using System.Web.WebPages;
using DataAnnotationsExtensions;

namespace Site.Models {
    [TableName("Producto")]
    [PrimaryKey("ProductoID")]
    [ExplicitColumns]
    public class Producto {

        [PetaPoco.Column("ProductoID")]
        [Display(Name="ID")]
        public int ProductoID { get; set; }
        
        [PetaPoco.Column("CodigoArticulo")]
        [Display(Name="Código Artículo")]
        public int? CodigoArticulo { get; set; }

        [PetaPoco.Column("PesoUnitario")]
        [Display(Name="Peso Unitario (Kg)")]
        [Range(0,100000)]
        public double PesoUnitario{ get; set; }

        [PetaPoco.Column("PrecioKg")]
        [Display(Name="Precio por Kg ($)")]
        [Range(0,100000)]
        public double PrecioKg{ get; set; }

        [PetaPoco.Column("PrecioUnitario")]
        [Display(Name="Precio Unitario ($)")]
        [Range(0,100000)]
        public double PrecioUnitario{ get; set; }

        [PetaPoco.Column("Descripcion")]
        [Display(Name="Descripción")]
		[DataType(DataType.MultilineText)]
        public string Descripcion{ get; set; }

        [PetaPoco.Column("UsuarioID")]
        [Display(Name="Proveedor")]
        [Required]
        public int ProveedorID { get; set; }

        [ResultColumn]
        [Display(Name = "Proveedor")]
        public String Proveedor { get; set; }

        [PetaPoco.Column("Notas")]
		[Display(Name = "Notas")]
		[DataType(DataType.MultilineText)]
		public String Notas{ get; set; }
        
        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
            sql.Append("Producto.*, usuario.Nombre As Proveedor");
            sql.Append("FROM Producto");
            sql.Append("INNER JOIN Usuario usuario ON Producto.UsuarioID=usuario.UsuarioID");
            return sql;
        }

        public static Producto SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE ProductoID = @0",id);
            return DbHelper.CurrentDb().SingleOrDefault<Producto>(sql);
        }


        public override string ToString() {
            if (!Descripcion.IsEmpty() && !CodigoArticulo.IsEmpty())
                return "Producto Nº "+CodigoArticulo.ToString()+" - "+ Descripcion;
            return Descripcion.IsEmpty()? (CodigoArticulo.HasValue?"Producto Nº"+CodigoArticulo.ToString():"Nuevo Producto"):Descripcion;
        }

        public bool IsValid(ModelStateDictionary state) {
            if (Descripcion.IsEmpty() && !CodigoArticulo.HasValue) {
                state.AddModelError("CodigoArticulo", "Debe especificarse el código de artículo o la descripción");
                return false;
            }
            if (Sitio.EsEmpleado) { 
                if ( ProveedorID.IsEmpty() ) {
                    state.AddModelError("ProveedorID", "Debe especificarse el proveedor del producto");
                    return false;
                }
            }
            if (!Usuario.HasRol(ProveedorID,"Proveedor")) {
                state.AddModelError("ProveedorID", "Debe especificarse un proveedor válido");
                return false;
            }
            else {
                if (ProveedorID != Sitio.Usuario.UsuarioID && !Sitio.EsEmpleado && ProveedorID != Sitio.Usuario.ProveedorID) {
                    state.AddModelError("ProveedorID", "No pueden registrarse productos de otros usuarios");
                    return false;
                }
            }
            if (ProductoID.IsEmpty()) { 
                var sql = PetaPoco.Sql.Builder.Append("SELECT * FROM Producto WHERE CodigoArticulo = @0 AND Convert(nvarchar(200), ISNULL(Descripcion,'') ) = @1 AND UsuarioID = @2",CodigoArticulo,Descripcion??"",ProveedorID);
                var sameProduct = DbHelper.CurrentDb().SingleOrDefault<Producto>(sql);
                if (sameProduct != null) { 
                    state.AddModelError("CodigoArticulo", "Ya existe un producto resgistrado con el mismo código de artículo y descripción para este usuario");
                    return false;
                }
            }
            return true;
        }

        public void Delete() {
            var db = DbHelper.CurrentDb();
            db.Execute("DELETE FROM Producto WHERE ProductoID = @0", ProductoID);
        }

        public bool HasUser(int UsuarioID) {
            return (this.ProveedorID == UsuarioID );
        }

        public bool IsNew(){
            return ProductoID.IsEmpty();
        }
                
        // podrá gestionar si tiene el permiso y, o es empleado o participa del evento (con el rol que puede editarlo).
        public bool CurrentUserCanAccessToFunction(Seguridad.Feature function) {
            return (Seguridad.CanAccessToFunction(Sitio.Usuario.UsuarioID,(int)Seguridad.Permisos.Cargamento,function) && (Sitio.EsEmpleado || HasUser(Sitio.Usuario.UsuarioID) || HasUser(Sitio.Usuario.ProveedorID??0)));
        }

    }
}