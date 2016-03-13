using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizLibMVC;

namespace Site.Models {
    public static class LoteSuperListHtmlHelper {
        public static IHtmlString LoteSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre) {
            return DoLoteSuperList(NombreControl, NombreControl, NombreControlNombre, NombreControlNombre, elID, elNombre);
        }

        public static IHtmlString LoteSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre, string NombreArray, int IndiceArray) {
            return DoLoteSuperList(string.Format("{0}_{1}__{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}_{1}__{2}", NombreArray, IndiceArray, NombreControlNombre),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControlNombre),
                elID, elNombre);
        }

        private static IHtmlString DoLoteSuperList(string IDControl, string NombreControl, string IDControlNombre, string NombreControlNombre, int? ID, string Nombre) {
            string retval = string.Format(@"
                    <span class='superlist'>
                    <input type='hidden' id='{0}' name='{1}' value='{2}'/>
                    <input type='text' id='{3}' name='{4}' value='{5}' style='Width: 100px;' class='superlistinput' data-controller='Lote' data-extrainfo='true' /></span>",
                        IDControl, NombreControl, (ID ?? 0).ToString(), IDControlNombre, NombreControlNombre, Nombre);

            retval = retval + string.Format("<a class='likelinknound noprint extrainfo masinfo' rel='/Lote/ExtraInfo/{0}' style='display: {1}; padding: 0 0 0 2px'><i class='icon-link'></i></a>", (ID ?? 0).ToString(), ID.IsEmpty() ? "none" : "inline");

            return MvcHtmlString.Create(retval);

        }
        
    }
}
