using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

namespace Site.Models {
    public class HorasRegimen {
        public int Codigo { set; get; }
        public string Nombre { set; get; }

        public static List<HorasRegimen> GetList(string IncludeAllItemstext) {
            List<HorasRegimen> retval = new List<HorasRegimen>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new HorasRegimen { Nombre = IncludeAllItemstext, Codigo = 0 });
            }
            retval.Add(new HorasRegimen { Codigo = 250, Nombre = "250" });
            retval.Add(new HorasRegimen { Codigo = 500, Nombre = "500" });
            retval.Add(new HorasRegimen { Codigo = 750, Nombre = "750" });
            retval.Add(new HorasRegimen { Codigo = 1000, Nombre = "1000" });
            retval.Add(new HorasRegimen { Codigo = 1250, Nombre = "1250" });
            retval.Add(new HorasRegimen { Codigo = 1500, Nombre = "1500" });
            retval.Add(new HorasRegimen { Codigo = 1750, Nombre = "1750" });
            retval.Add(new HorasRegimen { Codigo = 2000, Nombre = "2000" });
            return retval;
        }

        public static SelectList GetSelectList(int ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(HorasRegimen.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }
    }
}

