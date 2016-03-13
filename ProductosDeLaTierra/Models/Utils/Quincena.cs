using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

namespace Site.Models {
    public class Quincena {
        public int Codigo { set; get; }
        public string Nombre { set; get; }

        public static Quincena Primera = new Quincena() { Nombre = "1ra", Codigo = 1 };
        public static Quincena Segunda = new Quincena() { Nombre = "2da", Codigo = 2 };

        public static List<Quincena> GetList(string IncludeAllItemstext) {
            List<Quincena> retval = new List<Quincena>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new Quincena {Nombre = IncludeAllItemstext, Codigo = 0});
            }
            retval.Add(Primera);
            retval.Add(Segunda);
            return retval;
        }

        public static SelectList GetSelectList(int? ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(Quincena.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }


    }
}

