using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Site.Models
{
    public class IDNombrePar
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string ExtraInfo { get; set; }

        public IDNombrePar() { }
        public IDNombrePar(int ID, string Nombre) {
            this.ID = ID;
            this.Nombre = Nombre;
        }

    }
    
}