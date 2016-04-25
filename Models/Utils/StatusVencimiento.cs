using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

namespace Site.Models {
    public class StatusVencimiento {
        public string Codigo { set; get; }
        public string Nombre { set; get; }

        public static StatusVencimiento Vencidos = new StatusVencimiento { Codigo = "Vencidos", Nombre = "Vencidos" };
        public static StatusVencimiento Vigentes = new StatusVencimiento { Codigo = "Vigentes", Nombre = "Vigentes" };

        public static List<StatusVencimiento> GetList(string IncludeAllItemstext) {
            List<StatusVencimiento> retval = new List<StatusVencimiento>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new StatusVencimiento { Nombre = IncludeAllItemstext, Codigo = "" });
            }

            retval.Add(Vencidos);
            retval.Add(Vigentes);

            return retval;
        }

        public static SelectList GetSelectList(string ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(StatusVencimiento.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }


    }
}

