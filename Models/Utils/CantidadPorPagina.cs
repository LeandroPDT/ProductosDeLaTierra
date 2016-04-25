using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;


namespace Site.Models {
    public class CantidadPorPagina {
        public int Codigo { set; get; }
        public string Nombre { set; get; }

        public const string NoPermitirTodas = "NoPermitirTodas";

        public static List<CantidadPorPagina> GetList(string IncludeAllItemstext) {
            List<CantidadPorPagina> retval = new List<CantidadPorPagina>();
            retval.Add(new CantidadPorPagina { Codigo = 50, Nombre = "50" });
            retval.Add(new CantidadPorPagina { Codigo = 100, Nombre = "100" });
            retval.Add(new CantidadPorPagina { Codigo = 150, Nombre = "150" });
            retval.Add(new CantidadPorPagina { Codigo = 200, Nombre = "200" });
            retval.Add(new CantidadPorPagina { Codigo = 500, Nombre = "500" });
            //if (IncludeAllItemstext != NoPermitirTodas) {
            //    retval.Add(new CantidadPorPagina { Codigo = 0, Nombre = "Todas" });
            //}
            return retval;
        }

        public static SelectList GetSelectList(int ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(CantidadPorPagina.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }
    }
}

