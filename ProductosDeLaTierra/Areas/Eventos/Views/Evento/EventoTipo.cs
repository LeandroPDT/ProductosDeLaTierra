using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Site.Models {
    public class EventoTipo {

        public int ID { get; set; }
		public String Nombre { get; set; }
        public String ViewName { get; set; }
        public int Permiso { get; set; }
        public String EstadoInicial { get; set; }
        public String EstadoFinal { get; set; }
        public bool SumaMercaderia { get; set; }
        public bool TieneMercaderia { get; set; }
        public string RolNotificable { get; set; }

        public static EventoTipo Envio = new EventoTipo() { ID = 1, Nombre = "Envio", ViewName = "EditarEnvio",EstadoInicial="Enviado",EstadoFinal="Enviado", Permiso = (int)Seguridad.Permisos.EventoEnvio,SumaMercaderia=true,RolNotificable="Cliente",TieneMercaderia=true};
        public static EventoTipo Recepcion = new EventoTipo() { ID = 2, Nombre = "Recepcion", ViewName = "EditarRecepcion",EstadoInicial="Enviado",EstadoFinal="Recibido", Permiso = (int)Seguridad.Permisos.EventoRecepcion,SumaMercaderia=true,RolNotificable="Proveedor", TieneMercaderia = true };
        public static EventoTipo Venta = new EventoTipo() { ID = 3, Nombre = "Venta", ViewName = "EditarVenta",EstadoInicial="Recibido",EstadoFinal="Recibido", Permiso = (int)Seguridad.Permisos.EventoVenta,SumaMercaderia=false,RolNotificable="", TieneMercaderia = true };
        public static EventoTipo Decomisacion = new EventoTipo() { ID = 4, Nombre = "Decomisacion", ViewName = "EditarDecomisacion",EstadoInicial="Recibido",EstadoFinal="Recibido", Permiso = (int)Seguridad.Permisos.EventoDecomisacion,SumaMercaderia=false,RolNotificable="Proveedor", TieneMercaderia = true };
        public static EventoTipo Cobro = new EventoTipo() { ID = 5, Nombre = "Cobro", ViewName = "EditarCobro", EstadoInicial = "Vendido", EstadoFinal = "Vendido", Permiso = (int)Seguridad.Permisos.EventoCobro, SumaMercaderia = false, RolNotificable = "Proveedor", TieneMercaderia = false };

        public static List<EventoTipo> Lista (){
            var ListaResultado = new List<EventoTipo>();            
            ListaResultado.Add(Envio);                             
            ListaResultado.Add(Recepcion);                        
            ListaResultado.Add(Venta);                        
            ListaResultado.Add(Decomisacion);
            ListaResultado.Add(Cobro);
            return ListaResultado;
        }
        
        public static SelectList EventoTipoSelectList(String ValorActualSeleccionado) {
            var retval = new List<KeyValuePair<String,String>>();
            retval.Add(new KeyValuePair<String,String>("","") );
            foreach (EventoTipo tipo in Lista()) { 
                if (Seguridad.CanAccess(tipo.Permiso))
                    retval.Add(new KeyValuePair<String,String>(tipo.Nombre,tipo.Nombre));
            }
            return new SelectList(retval, "Key", "Value", ValorActualSeleccionado);
        }

        public EventoTipo() {
        }

        public EventoTipo(int ID) {
            var o = (from rec in Lista() where rec.ID == ID select rec).SingleOrDefault() ?? new EventoTipo();
            this.ID = o.ID;
            this.Nombre = o.Nombre;
            this.ViewName = o.ViewName;
            this.Permiso = o.Permiso;
            this.SumaMercaderia = o.SumaMercaderia;
            this.EstadoInicial = o.EstadoInicial;
            this.EstadoFinal = o.EstadoFinal;
            this.RolNotificable = o.RolNotificable;
            this.TieneMercaderia = o.TieneMercaderia;
        }

        public EventoTipo(string type) {
            var o = (from rec in Lista() where rec.Nombre == type select rec).SingleOrDefault() ?? new EventoTipo();
            this.ID = o.ID;
            this.Nombre = o.Nombre;
            this.ViewName = o.ViewName;
            this.Permiso = o.Permiso;
            this.SumaMercaderia = o.SumaMercaderia;
            this.EstadoInicial = o.EstadoInicial;
            this.EstadoFinal = o.EstadoFinal;
            this.RolNotificable = o.RolNotificable;
            this.TieneMercaderia = o.TieneMercaderia;
        }

        public string DefaultReturnAction {
            get {
                return this.Nombre;
            }
        }


    }
}