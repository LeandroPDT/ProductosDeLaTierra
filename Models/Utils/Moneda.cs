using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

namespace Site.Models {
    public class Moneda {
        public string Codigo { set; get; }
        public string Nombre { set; get; }

        public static Moneda Dolar = new Moneda() { Nombre = "Dólar", Codigo = "D" };
        public static Moneda Peso = new Moneda() { Nombre = "Peso", Codigo = "P" };

        public static List<Moneda> GetList(string IncludeAllItemstext)         {
            List<Moneda> retval = new List<Moneda>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new Moneda {Codigo="", Nombre = IncludeAllItemstext});
            }
            retval.Add(Dolar);
            retval.Add(Peso);
            return retval;
        }

        public static SelectList GetSelectList(string ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(Moneda.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }


    }
}

