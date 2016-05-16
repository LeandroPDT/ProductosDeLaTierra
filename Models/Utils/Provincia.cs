using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

namespace Site.Models
{
    public class Provincia {
        public string Codigo { set; get; }
        public string Nombre { set; get; }

        public const string Exterior = "Otra - Exterior";

        public static List<Provincia> GetList(string IncludeAllItemstext) {
            List<Provincia> retval = new List<Provincia>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new Provincia {Nombre = IncludeAllItemstext});
            }
            retval.Add(new Provincia { Nombre = "Ciudad de Buenos Aires" });
            retval.Add(new Provincia {Nombre = "Buenos Aires" });
            retval.Add(new Provincia {Nombre = "Catamarca" });
            retval.Add(new Provincia {Nombre = "Chaco"});
            retval.Add(new Provincia {Nombre = "Chubut"});
            retval.Add(new Provincia {Nombre = "Córdoba"});
            retval.Add(new Provincia {Nombre = "Corrientes" });
            retval.Add(new Provincia {Nombre = "Entre Ríos"});
            retval.Add(new Provincia {Nombre = "Formosa"});
            retval.Add(new Provincia {Nombre = "Jujuy"});
            retval.Add(new Provincia {Nombre = "La Pampa" });
            retval.Add(new Provincia {Nombre = "La Rioja" });
            retval.Add(new Provincia {Nombre = "Mendoza"});
            retval.Add(new Provincia {Nombre = "Misiones" });
            retval.Add(new Provincia {Nombre = "Neuquén"});
            retval.Add(new Provincia {Nombre = "Río Negro"});
            retval.Add(new Provincia {Nombre = "Salta"});
            retval.Add(new Provincia {Nombre = "San Juan"});
            retval.Add(new Provincia {Nombre = "San Luis" });
            retval.Add(new Provincia {Nombre = "Santa Cruz"});
            retval.Add(new Provincia {Nombre = "Santa Fé"});
            retval.Add(new Provincia {Nombre = "Santiago Del Estero" });
            retval.Add(new Provincia {Nombre = "Tierra Del Fuego" });
            retval.Add(new Provincia {Nombre = "Tucumán"});
            retval.Add(new Provincia {Nombre = Exterior });
            return retval;
        }

        public static SelectList GetSelectList(string ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(Provincia.GetList(IncludeAllItemstext), "Nombre", "Nombre", ValorActualSeleccionado);
        }


    }
}

