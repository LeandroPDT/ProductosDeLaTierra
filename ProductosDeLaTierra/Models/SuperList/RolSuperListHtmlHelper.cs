using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Models {
    public static class RolSuperListHtmlHelper {
        public static IHtmlString RolSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre) {
            return DoRolSuperList(NombreControl, NombreControl, NombreControlNombre, NombreControlNombre, elID, elNombre);
        }

        public static IHtmlString RolSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre, string NombreArray, int IndiceArray) {
            return DoRolSuperList(string.Format("{0}_{1}__{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}_{1}__{2}", NombreArray, IndiceArray, NombreControlNombre),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControlNombre),
                elID, elNombre);
        }

        private static IHtmlString DoRolSuperList(string IDControl, string NombreControl, string IDControlNombre, string NombreControlNombre, int? ID, string Nombre) {
            string retval = string.Format(@"
                    <span class='superlist'>
                    <input type='hidden' id='{0}' name='{1}' value='{2}'/>
                    <input type='text' id='{3}' name='{4}' value='{5}' style='Width: 250px;' class='superlistinput' data-controller='config/roles' /></span>", IDControl, NombreControl, (ID ?? 0).ToString(), IDControlNombre, NombreControlNombre, Nombre);
            return MvcHtmlString.Create(retval);
        }

    }
}