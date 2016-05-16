using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Models {
    public static class UsuarioSuperListHtmlHelper {
        public static IHtmlString UsuarioSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre,Options options= null)  {
            return DoUsuarioSuperList(NombreControl, NombreControl, NombreControlNombre, NombreControlNombre, elID, elNombre,options);
        }

        public static IHtmlString UsuarioSuperList(this HtmlHelper htmlHelper, string NombreControl, string NombreControlNombre, int? elID, string elNombre, string NombreArray, int IndiceArray,Options options= null) {
            //Name = ArrayName + "_" + ArrarIndex.ToString + "__" + ControlName
            //NameForName = ArrayName + "[" + ArrarIndex.ToString + "]." + ControlName

            return DoUsuarioSuperList(string.Format("{0}_{1}__{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControl),
                string.Format("{0}_{1}__{2}", NombreArray, IndiceArray, NombreControlNombre),
                string.Format("{0}[{1}].{2}", NombreArray, IndiceArray, NombreControlNombre),
                elID, elNombre);
        }


        private static IHtmlString DoUsuarioSuperList(string IDControl, string NombreControl, string IDControlNombre, string NombreControlNombre, int? ID, string Nombre,Options options= null) {
            var opt = options ?? new Options();
            
            string retval = string.Format(@"
                    <span class='superlist'>
                    <input type='hidden' id='{0}' name='{1}' value='{2}'/>
                    <input type='text' placeholder='Por Nombre, Username ó Email' id='{3}' name='{4}' value='{5}' style='Width: 200px;' class='usuariolistinput' data-controller='config/Usuario' data-params='{{""Rol"":""{6}""}}' /></span>", IDControl, NombreControl, (ID ?? 0).ToString(), IDControlNombre, NombreControlNombre, Nombre,opt.Rol);
            return MvcHtmlString.Create(retval);

        }

        public class Options {
            public String Rol { get; set; }
        }

    }
}