using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// Una clase que te maneja una lista unica de patentes y permite facilmente agregar y formatear las mismas
namespace Site.Models {
    public class ListaPatentes {
        private List<String> _lista;
        public ListaPatentes() {
            _lista = new List<string>();
        }

        public void Add(string ListaPatentesSeparadasPorComa) {
            if (!string.IsNullOrEmpty(ListaPatentesSeparadasPorComa)) { 
                string[] Patentes = ListaPatentesSeparadasPorComa.Split(',');
                foreach (string p in Patentes) {
                    var Patente = p.Replace(" ", "").ToUpper();
                    if (Patente.IsValidPatente()) { 
                        if (!_lista.Contains(Patente)) {
                            _lista.Add(Patente);
                        }
                    }
                }
            }
        }

        public override string ToString() {
            return string.Join(", ", _lista);
        }

    }
}