using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizLibMVC;

namespace Site.Models {
    public static class CargamentoSuperListHtmlHelper {
        public static IHtmlString CargamentoSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre, Options Options = null) {
            return DoCargamentoSuperList(NombreControl, NombreControl, NombreControlNombre, NombreControlNombre, elID, elNombre, Options);
        }

        public static IHtmlString CargamentoSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre, string NombreArray, int IndiceArray, Options Options = null) {
            return DoCargamentoSuperList(string.Format("{0}_{1}__{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}_{1}__{2}", NombreArray, IndiceArray, NombreControlNombre),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControlNombre),
                elID, elNombre, Options);
        }

        private static IHtmlString DoCargamentoSuperList(string IDControl, string NombreControl, string IDControlNombre, string NombreControlNombre, int? ID, string Nombre, Options Options = null) {
            Options = Options ?? new Options();
            string retval = string.Format(@"
                    <span class='superlist'>                    
                    <input type='hidden' id='{0}' name='{1}' value='{2}'/>
                    <input type='text' id='{3}' name='{4}' value='{5}' style='Width: 150px;' class='cargamentolistinput' data-controller='Cargamento'  data-params='{{""Estado"":""{6}"",""ProveedorID"":""{7}""}}'/></span>",
                        IDControl, NombreControl,  ID>0 ? ID.ToString():"", IDControlNombre, NombreControlNombre, Nombre,Options.Estado, Options.ProveedorID.ToString());

            //retval = retval + string.Format("<a class='likelinknound noprint extrainfo masinfo' rel='/Cargamento/ExtraInfo/{0}' style='display: {1}; padding: 0 0 0 2px'><i class='icon-link'></i></a>", (ID ?? 0).ToString(), ID.IsEmpty() ? "none" : "inline");

            return MvcHtmlString.Create(retval);

        }
        
        public class Options {
            public int? ProveedorID { get; set; }
            public string Estado { get; set; }

            public Options() {
                Estado = "";
                ProveedorID = null;
            }

        }
    }
}
