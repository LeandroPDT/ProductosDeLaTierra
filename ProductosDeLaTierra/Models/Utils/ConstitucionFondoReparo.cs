using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

//public const int FR_CONSTITUIDO_EN_EFECTIVO = 1                       ;
//public const int FR_CONSTITUIDO_EN_POLIZA = 2                         ;
//public const int FR_CONSTITUIDO_EN_AVAL = 3                           ;


namespace Site.Models {
    public class ConstitucionFondoReparo {
        public int Codigo { set; get; }
        public string Nombre { set; get; }

        public static ConstitucionFondoReparo Efectivo = new ConstitucionFondoReparo() { Nombre = "Efectivo", Codigo = 1 };
        public static ConstitucionFondoReparo Poliza = new ConstitucionFondoReparo() { Nombre = "Póliza", Codigo = 2 };
        public static ConstitucionFondoReparo Aval = new ConstitucionFondoReparo() { Nombre = "Aval", Codigo = 3 };

        public static List<ConstitucionFondoReparo> GetList(string IncludeAllItemstext) {
            List<ConstitucionFondoReparo> retval = new List<ConstitucionFondoReparo>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new ConstitucionFondoReparo {Nombre = IncludeAllItemstext, Codigo = 0});
            }
            retval.Add(Efectivo);
            retval.Add(Poliza);
            retval.Add(Aval);
            return retval;
        }

        public static SelectList GetSelectList(int? ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(ConstitucionFondoReparo.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }

        public static string NombreForCodigo(int Codigo) {
            return (from rec in GetList("") where rec.Codigo == Codigo select rec.Nombre).FirstOrDefault();
        }



    }
}

