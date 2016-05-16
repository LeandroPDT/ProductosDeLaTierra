using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BizLibMVC;
using BizLibMVC.HtmlHelpers;
using System.Web.WebPages;
using System.Web.Mvc;

namespace Site.Models {
    public class TipoIncidente {
        public string Codigo { set; get; }
        public string Nombre { set; get; }

        public static TipoIncidente Feature = new TipoIncidente { Codigo = "Feature", Nombre = "Mejora" };
        public static TipoIncidente Bug = new TipoIncidente { Codigo = "Bug", Nombre = "Error o falla" };

        public static List<TipoIncidente> GetList(string IncludeAllItemstext)
        {
            List<TipoIncidente> retval = new List<TipoIncidente>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new TipoIncidente { Nombre = IncludeAllItemstext, Codigo = "" });
            }

            retval.Add(Feature);
            retval.Add(Bug);

            return retval;
        }

        public static SelectList GetSelectList(string ValorActualSeleccionado, string IncludeAllItemstext = "")
        {
            return new SelectList(TipoIncidente.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }

        public static string NombreForCodigo(string Codigo) {
            return (from rec in GetList("") where rec.Codigo == Codigo select rec.Nombre).FirstOrDefault();
        }


    }
}

