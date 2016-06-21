using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;
using System.Web.WebPages;
using DataAnnotationsExtensions;

namespace Site.Models {
    [TableName("Cargamento")]
    [PrimaryKey("CargamentoID")]
    [ExplicitColumns]
    public class Cargamento {

        [PetaPoco.Column("CargamentoID")]
        [Display(Name="ID")]
        public int CargamentoID { get; set; }

        [PetaPoco.Column("NumeroRemito")]
        [Display(Name="Nro. Remito")]
        [Required]
        public int NumeroRemito { get;set;}

        //private int _NumeroRemito;

        //[PetaPoco.Column("NumeroRemito")]
        //[Display(Name="Nro. Remito:")]
        //[Required]
        //public int NumeroRemito { 
        //    get{
        //        return _NumeroRemito;
        //    }
        //    set{
        //        _NumeroRemito = value;
        //        Referencia = String.Concat("000",value.ToString()).Substring(value.ToString().Length+3-4) + "-" + FechaEnvio.Year.ToString().Substring(2) + String.Concat(FechaEnvio.Month.ToString().Length==2?"":"0" ,FechaEnvio.Month.ToString()) + String.Concat(FechaEnvio.Day.ToString().Length==2?"":"0" ,FechaEnvio.Day.ToString());
        //    }
        //}

        //[PetaPoco.Column("Referencia")]
        //[Display(Name = "Referencia")]
        //public String Referencia { get; set; }
        
		[PetaPoco.Column("FechaEnvio")]
		[Display(Name = "Fecha de Envío")]
		[DataType(DataType.DateTime)]
        [Required]
		public DateTime FechaEnvio { get; set; }
        
        [PetaPoco.Column("MercaderiaID")]
        [Required]
        public int MercaderiaID { get; set; }
        
        // virtual proxy de la mercadería
        private Mercaderia _Mercaderia;
        public Mercaderia Mercaderia{ 
            get{
                if (_Mercaderia == null ) {
                    _Mercaderia = MercaderiaID.IsEmpty()? new Mercaderia():Mercaderia.SingleOrDefault(MercaderiaID)??new Mercaderia();
                }
                return _Mercaderia;
            }
            set {
                _Mercaderia = value;
            }
        }
        
        [PetaPoco.Column("ProveedorID")]
        [Required]
        public int ProveedorID { get; set; }
        [PetaPoco.Column("ClienteID")]
        [Required]
        public int ClienteID { get; set; }

        [PetaPoco.Column("Ganancia")]
        [Display(Name="Ganancia ($)")]
        [Range(0,100000)]
        [Required]
        public double Ganancia{ get; set; }

        [PetaPoco.Column("CostoFlete")]
        [Display(Name="Costo del flete ($)")]
        [Range(0,100000)]
        [Required]
        public double CostoFlete{ get; set; }

        [PetaPoco.Column("CostoDescarga")]
        [Display(Name="Costo de la descarga ($)")]
        [Range(0,100000)]
        [Required]
        public double CostoDescarga{ get; set; }

        [ResultColumn]
        [Display(Name = "Proveedor")]
        public String Proveedor { get; set; }

        [ResultColumn]
        [Display(Name = "Cliente")]
        public String Cliente { get; set; }
        
        [PetaPoco.Column("Recibido")]
        [Display(Name = "Recibido")]
        public bool Recibido { get; set; }

        [PetaPoco.Column("Vendido")]
        [Display(Name = "Vendido")]
        [Required]
        public bool Vendido { get; set; }
        
        [ResultColumn]
        [Display(Name="Estado")]
        public String Estado {
            get {
                return Vendido ? "Vendido" : Recibido ? "Recibido" : "Enviado";
            }
            set {
                Vendido = (value == "Vendido");
                Recibido = (value == "Vendido") || (value == "Recibido");
            }
        }

        public bool EnEstadoFinal {
            get {
                return Vendido;
            }
        }
        
        [PetaPoco.Column("PrecioCerrado")]
        [Display(Name = "Por Precio Cerrado")]
        [Required]
        public bool PrecioCerrado { get; set; }
        
        [ResultColumn]
		[Display(Name = "Tipo de venta")]
        public String TipoVenta { 
            get { return PrecioCerrado ? "Precio Cerrado" : "En Consignación"; }
            set {
                PrecioCerrado = (value == "Precio Cerrado");
            }
        }

        
        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
            sql.Append("Cargamento.*, usuario1.Nombre As Proveedor, usuario2.Nombre As Cliente");
            sql.Append("FROM Cargamento");
            sql.Append("INNER JOIN Usuario usuario1 ON Cargamento.ProveedorID = usuario1.UsuarioID");
            sql.Append("INNER JOIN Usuario usuario2 ON Cargamento.ClienteID = usuario2.UsuarioID");
            return sql;
        }

        public static Cargamento SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE CargamentoID = @0",id);
            return DbHelper.CurrentDb().SingleOrDefault<Cargamento>(sql);
        }


        public override string ToString() {
            return IsNew() ? "Nuevo Cargamento" : "Cargamento Nro. " + NumeroRemito.ToString();// +" - enviado el " + FechaEnvio.ToShortDateString();    
        }

        public bool IsValid(ModelStateDictionary state) {
            if (NumeroRemito.IsEmpty()) {
                state.AddModelError("", "Debe especificarse el número de remito del cargamento");
                return false;
            }
            
            if (FechaEnvio.IsEmpty()) {
                state.AddModelError("FechaEnvio", "Debe especificarse la fecha de envío del cargamento");
                return false;
            }
                      
            if ((from IDNombrePar par in Usuario.RolUserIDList() where par.ID == ProveedorID && par.Nombre=="Proveedor" select par).ToList().Count == 0) {
                state.AddModelError("ProveedorID", "Debe especificarse un proveedor válido");
                return false;
            }   
            
            if ((from IDNombrePar par in Usuario.RolUserIDList() where par.ID == ClienteID && par.Nombre=="Cliente" select par).ToList().Count == 0) {
                state.AddModelError("ClienteID", "Debe especificarse un cliente válido");
                return false;
            }
            if (Sitio.EsEmpleado) { 
                if ( ProveedorID.IsEmpty() ) {
                    state.AddModelError("ProveedorID", "Debe especificarse el proveedor del nuevo Cargamento");
                    return false;
                }
            }
            else {
                if (IsNew() && ProveedorID != Sitio.Usuario.UsuarioID && ProveedorID!=Sitio.Usuario.ProveedorID) {
                    state.AddModelError("ProveedorID", "No pueden registrarse Cargamentos de otros proveedores");
                    return false;
                }
            }
            if (CargamentoID.IsEmpty()) {
                var sql = PetaPoco.Sql.Builder.Append("SELECT * FROM Cargamento WHERE NumeroRemito = @0 AND FechaEnvio = @1 AND ProveedorID = @2 AND ClienteID = @3",NumeroRemito,FechaEnvio,ProveedorID,ClienteID);
                List<Cargamento> sameProduct = DbHelper.CurrentDb().Fetch<Cargamento>(sql);
                if (sameProduct != null && sameProduct.Count>0) { 
                    state.AddModelError("CodigoArticulo", "Ya existe un cargamento resgistrado con el mismo número de remito, cliente y fecha de envío para este usuario");
                    return false;
                }
            }            
            return true;
        }

        // únicamente guarda el estado, usable en transacciones mas grandes.
        public virtual void DoSave() {
            BeforeSave();
            DbHelper.CurrentDb().SaveAndLog(this);
        }

        public virtual void BeforeSave() {
            // si aún no se guardo la mercadería(no se tiene su id) y esta contiene elementos (caso del envío), debe guardarse.
            if (Mercaderia.HasItems()) {
                Mercaderia.DoSave();
                MercaderiaID = Mercaderia.MercaderiaID;
            }
        }  

        public void Delete() {
            var db = DbHelper.CurrentDb();
            using (var scope = db.GetTransaction()) {
                // desvinculo los objetos relacionados al evento
                BeforeDelete();
                DoDelete();
                scope.Complete();
            } 
        }

        public void DoDelete() {
            var db = DbHelper.CurrentDb();
            db.Execute("DELETE FROM Cargamento WHERE CargamentoID = @0", this.CargamentoID);
            AfterDelete();
        }


        public virtual void BeforeDelete() {
            var db = DbHelper.CurrentDb();
            var sql = Evento.BaseQuery().Append("WHERE CargamentoID = @0", CargamentoID);
            sql.Append("ORDER BY Fecha desc");
            var objectEvents = DbHelper.CurrentDb().Fetch<Evento>(sql);
            foreach (Evento e in objectEvents) {
                e.DoDelete();
                e.AfterDelete();
            }
        }

        public void AfterDelete() {            
            new Mercaderia() { MercaderiaID = MercaderiaID }.DoDelete();
        }

        public static void AppendTipoVentaMatching(Sql sql, string TipoVenta) {
            if (!TipoVenta.IsEmpty()) {
                switch (TipoVenta) {
                    case "Precio Cerrado":
                        sql.Append("AND Cargamento.PrecioCerrado= 1");
                        break;
                    case "En Consignación":
                        sql.Append("AND Cargamento.PrecioCerrado = 0");
                        break;
                }
            }
        }

        public static void AppendEstadoMatching(Sql sql,string Estado) {
            if (!Estado.IsEmpty()) {
                EstadoCargamento estadoBuscado = new EstadoCargamento(Estado);
                sql.Append(String.Concat("AND Cargamento.Recibido = ", estadoBuscado.implicaRecibido==true?"1":"0"));
                sql.Append(String.Concat("AND Cargamento.Vendido = ", estadoBuscado.implicaVendido==true?"1":"0"));
            }
        }

        public bool HasUser(int UsuarioID) {
            return (this.ProveedorID == UsuarioID || this.ClienteID == UsuarioID);
        }

        public bool IsNew(){
            return CargamentoID.IsEmpty();
        }

        public List<Observacion> Observaciones {
            get {
                var sql = Evento.BaseQuery();
                sql.Append("WHERE CargamentoID = @0 AND Notas IS NOT NULL", this.CargamentoID);
                sql.Append("ORDER BY Fecha ASC");
                var result = DbHelper.CurrentDb().Fetch<Evento>(sql);
                var retval = new List<Observacion>();
                foreach(Evento evento in result) {
                    if (!evento.Notas.IsEmpty())
                    {
                        retval.Add(new Observacion() { Texto = evento.Tipo + " del " + evento.Fecha.ToShortDateString(), Cuerpo = evento.Notas });
                    }
                }
                return retval;
            }
        }

        public class Observacion{
            public String Texto { get; set; }
            public String Cuerpo { get; set; }
        }

        // podrá gestionar si tiene el permiso y, o es empleado o participa del evento (con el rol que puede editarlo).
        public bool CurrentUserCanAccessToFunction(Seguridad.Feature function) {
            return (Seguridad.CanAccessToFunction(Sitio.Usuario.UsuarioID,(int)Seguridad.Permisos.Cargamento,function) && (Sitio.EsEmpleado || HasUser(Sitio.Usuario.UsuarioID)||HasUser(Sitio.Usuario.ProveedorID??0)));
        }
        
        public static SelectList TipoVentaSelectList(String ValorActualSeleccionado) {
            var Lista = new List<KeyValuePair<String,String>>();
            Lista.Add(new KeyValuePair<String,String>("","") );  
            Lista.Add(new KeyValuePair<String,String>("En Consignación","En Consignación") );   
            Lista.Add(new KeyValuePair<String,String>("Precio Cerrado","Precio Cerrado") ); 
            return new SelectList(Lista, "Key", "Value", ValorActualSeleccionado);
        }

        public static SelectList EstadoSelectList(String ValorActualSeleccionado) {
            var Lista = new List<KeyValuePair<String,String>>();
            Lista.Add(new KeyValuePair<String,String>("","") );  
            foreach (EstadoCargamento estado in new EstadoCargamento().laLista())
                Lista.Add(new KeyValuePair<String,String>(estado.nombre,estado.nombre));
            return new SelectList(Lista, "Key", "Value", ValorActualSeleccionado);
        }

        // El cargamento puede estar en 3 estados: Enviado (aun no se vendio ni recibio), Recibido (se recibio pero no se vendo totalmente) o Vendido (se recibio y se vendio)
        public class EstadoCargamento {
            public string nombre {get;set;}
            public bool implicaRecibido {get;set;}
            public bool implicaVendido {get;set;}
            public bool implicaEnviado {
                get{
                    return implicaRecibido ? false : implicaVendido ? false : true;
                }
                set{
                    if (value == true) {
                        implicaVendido = false;
                        implicaRecibido = false;
                    }
                }
            }

            public EstadoCargamento(string nombre) {
                var o = (from rec in Lista() where nombre == rec.nombre select rec).SingleOrDefault();
                this.nombre=o.nombre;
                this.implicaVendido=o.implicaVendido;
                this.implicaRecibido=o.implicaRecibido;
            }

            public EstadoCargamento() {

            }
            
            public List<EstadoCargamento> laLista() {
                return Lista();
            }

            public static List<EstadoCargamento> Lista() {
                var retval = new List<EstadoCargamento>();
                retval.Add( new EstadoCargamento(){nombre="Enviado",implicaRecibido=false,implicaVendido=false});
                retval.Add( new EstadoCargamento(){nombre="Recibido",implicaRecibido=true,implicaVendido=false});
                retval.Add( new EstadoCargamento(){nombre="Vendido",implicaRecibido=true,implicaVendido=true});
                return retval;
            }

        }
        
    }
}