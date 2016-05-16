using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizLibMVC;

namespace Site.Models {
    public static class ProductoSuperListHtmlHelper {
        public static IHtmlString ProductoSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre, Options options = null) {
            return DoProductoSuperList(NombreControl, NombreControl, NombreControlNombre, NombreControlNombre, elID, elNombre, options);
        }

        public static IHtmlString ProductoSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre, string NombreArray, int IndiceArray, Options options = null) {
            return DoProductoSuperList(string.Format("{0}_{1}__{2}", NombreArray.Replace(".","_"), IndiceArray, NombreControl),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}_{1}__{2}", NombreArray.Replace(".","_"), IndiceArray, NombreControlNombre),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControlNombre),
                elID, elNombre, options);
        }

        private static IHtmlString DoProductoSuperList(string IDControl, string NombreControl, string IDControlNombre, string NombreControlNombre, int? ID, string Nombre, Options options = null) {

            options = options ?? new Options();

            string retval = string.Format(@"
                    <span class='superlist'>                    
                    <input type='hidden' id='{0}' name='{1}' value='{2}'/>
                    <input type='text' placeholder='Por Código de Artículo ó Descripción' id='{3}' name='{4}' value='{5}' style='Width: 200px;' class='productolistinput' data-controller='Producto' data-params='{{""ProveedorID"":""{6}""}}' /></span>",
                        IDControl, NombreControl,  ID>0 ? ID.ToString():"", IDControlNombre,NombreControlNombre, Nombre,options.ProveedorID);

            //retval = retval + string.Format("<a class='likelinknound noprint extrainfo masinfo' rel='/Producto/ExtraInfo/{0}' style='display: {1}; padding: 0 0 0 2px'><i class='icon-link'></i></a>", (ID ?? 0).ToString(), ID.IsEmpty() ? "none" : "inline");

            return MvcHtmlString.Create(retval);

        }

        public class Options {
            public int? ProveedorID { get; set; }

            public Options() {
                
            }

            public Options(int ProveedorID) {
                this.ProveedorID = ProveedorID;
            }

        }
    }
}
