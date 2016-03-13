using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

namespace Site.Models {
    public class CotizaPor {

        public const int Hora = 1;
        public const int Mes = 2;

        public int Codigo { set; get; }
        public string Nombre { set; get; }

        public static List<CotizaPor> GetList(string IncludeAllItemstext) {
            List<CotizaPor> retval = new List<CotizaPor>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new CotizaPor { Nombre = IncludeAllItemstext, Codigo = 0 });
            }
            retval.Add(new CotizaPor { Codigo = Hora, Nombre = "Hora" });
            retval.Add(new CotizaPor { Codigo = Mes, Nombre = "Mes" });
            return retval;
        }

        public static SelectList GetSelectList(int ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(CotizaPor.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }
    }
}

