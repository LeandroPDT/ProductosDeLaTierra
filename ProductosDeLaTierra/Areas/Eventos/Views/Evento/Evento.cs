using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Reflection;

namespace Site.Models {
    [TableName("Evento")]
    [PrimaryKey("EventoID")]
    [ExplicitColumns]
    public class Evento {
        [PetaPoco.Column("EventoID")]
		[Display(Name = "Número de Evento")]
		public int EventoID{ get; set; }
        
        [PetaPoco.Column("TipoEventoID")]
		[Display(Name = "Tipo Evento")]
        [Required]
		public int TipoEventoID{ get; set; }
        
		[PetaPoco.Column("Fecha")]
		[Display(Name = "Fecha")]
		[DataType(DataType.DateTime)]
        [Required]
		public DateTime Fecha { get; set; }
        
        [PetaPoco.Column("CargamentoID")]
		public int? CargamentoID{ get; set; }
        
        [PetaPoco.Column("MercaderiaID")]
		public int? MercaderiaID{ get; set; }
                
		[PetaPoco.Column("Ganancia")]
		[Display(Name = "Ganancia ($)")]
		[DataType(DataType.Currency)]
        [Required]
		public Decimal Ganancia { get; set; }

        [PetaPoco.Column("Notas")]
		[Display(Name = "Notas")]
		[DataType(DataType.MultilineText)]
		public String Notas{ get; set; }
        
        // virtual proxy de la mercadería
        private Mercaderia _Mercaderia;
        public Mercaderia Mercaderia{ 
            get{
                if (_Mercaderia == null ) {
                    _Mercaderia = MercaderiaID.IsEmpty()? new Mercaderia():Mercaderia.SingleOrDefault(MercaderiaID ?? 0)??new Mercaderia();
                }
                return _Mercaderia;
            }
            set {
                _Mercaderia = value;
            }
        }
         
        // virtual proxy del cargamento
        private Cargamento _Cargamento;
        public Cargamento Cargamento{ 
            get{
                if (_Cargamento == null ) {
                    _Cargamento = CargamentoID.IsEmpty()?new Cargamento() :Cargamento.SingleOrDefault(CargamentoID ?? 0)??new Cargamento();
                }
                return _Cargamento;
            }
            set {
                _Cargamento = value;
            }
        }

        [ResultColumn]
        public String Tipo {
            get {
                var tipo = new EventoTipo(TipoEventoID);
                tipo.Nombre = tipo.Nombre == "Envio" ? "Envío" : tipo.Nombre;
                tipo.Nombre = tipo.Nombre == "Recepcion" ? "Recepción" : tipo.Nombre;
                tipo.Nombre = tipo.Nombre == "Decomisacion" ? "Decomisación" : tipo.Nombre;
                return tipo.Nombre;
            }
        }

        public Evento() {
            Fecha = DateTime.Now;
        }

		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
		    sql.Append("Evento.*");
		    sql.Append("FROM Evento");
		    return sql;
		}

        public override string ToString() {
            return (!CargamentoID.IsEmpty() && !Tipo.IsEmpty() ? Tipo+" "+Cargamento.ToString():"Nuevo "+Tipo  );
        }

        public static Evento SingleOrDefault(int id) {           
		    var sql = BaseQuery();
		    sql.Append("WHERE EventoID = @0", id);
            return DbHelper.CurrentDb().SingleOrDefault<Evento>(sql); ;
        }

        public static Evento SingleOrDefault( int EventoTipoID , int CargamentoID) {           
		    var sql = BaseQuery();
		    sql.Append("WHERE TipoEventoID = @0 AND CargamentoID = @1", EventoTipoID,CargamentoID);
            return DbHelper.CurrentDb().SingleOrDefault<Evento>(sql);
        }

        
        public virtual bool IsValid(ModelStateDictionary ModelState) {
            CargamentoID = CargamentoID.ZeroToNull();
            MercaderiaID = MercaderiaID.ZeroToNull();
            if (Fecha > DateTime.Now) {
                ModelState.AddModelError("FechaEnvio", "Debe especificar una fecha que no sea posterior a hoy");
                return false;
            }
            if (Cargamento.IsValid(ModelState) && Mercaderia.IsValid(ModelState) && validateRelatedObjects(ModelState)) {
                string nombreEvento = new EventoTipo(TipoEventoID).Nombre;
                string estadoInicialValido=new EventoTipo(TipoEventoID).EstadoInicial;
                if (IsNew() && (Cargamento.Estado != estadoInicialValido )) {
                    ModelState.AddModelError("NumeroRemito", "El cargamento debe estar "+estadoInicialValido+" para registrar su "+nombreEvento);
                    return false;
                }
                string estadoFinalValido=new EventoTipo(TipoEventoID).EstadoFinal;
                if (!IsNew() && (Cargamento.Estado != estadoFinalValido && !Cargamento.EnEstadoFinal)) {
                    ModelState.AddModelError("NumeroRemito", "El cargamento debe estar "+estadoFinalValido+" para editar su "+nombreEvento);
                    return false;
                }
                return true;
            }
            return false;
        }

        /* No lo ncesitamos todavia
        public Evento ToEvento() {
            return this;
        }

        public T FromEvento<T>(Evento otroEvento) {
            // nos evita asignar uno por uno los atributos.
            AutoMapper.Mapper.CreateMap<Evento, T>();
            return AutoMapper.Mapper.Map<T>(otroEvento);
        }
        */
        public void Save() {
            var db = DbHelper.CurrentDb();
            using (var scope = db.GetTransaction()) {
				// permito que las clases derivadas guarden lo que necesiten.
                BeforeSave();
                db.SaveAndLog(this);
                scope.Complete();
            } 
        }      

        public virtual void BeforeSave() {

            updateRelatedObjects();
            Cargamento.DoSave();
            CargamentoID = Cargamento.CargamentoID;
            Mercaderia.DoSave();
            MercaderiaID = Mercaderia.MercaderiaID;
            
        }

        private bool validateRelatedObjects(ModelStateDictionary state) {
            Mercaderia oldMercaderia = IsNew()?new Mercaderia():Mercaderia.SingleOrDefault(MercaderiaID??0)?? new Mercaderia();
            bool eventoSumaMercaderia = new EventoTipo(TipoEventoID).SumaMercaderia;
            if (!eventoSumaMercaderia && (Cargamento.Mercaderia.Peso < (Mercaderia.Peso-oldMercaderia.Peso))) {
                state.AddModelError("Mercaderia.Peso","El peso de la mercadería no puede ser mayor al peso de la mercadería del cargamento");
                return false;
            }
            if (!eventoSumaMercaderia && (Cargamento.Mercaderia.Bultos < (Mercaderia.Bultos-oldMercaderia.Bultos))) {
                state.AddModelError("Mercaderia.Bultos","Los bultos de la mercadería no pueden ser mayores a los bultos de la mercadería del cargamento");
                return false;
            }
            foreach (ItemMercaderia im in Mercaderia.Mercaderias) {
                if (im.ProveedorID != Cargamento.ProveedorID) {
                    state.AddModelError("Mercaderia.Mercaderias_" + Mercaderia.Mercaderias.IndexOf(im) + "__Descripcion", "No pueden agregarse artículos a las mercaderías que no sean del proveedor del cargamento");
                    return false;
                }
                List<ItemMercaderia> imsWithSameProduct = (from ItemMercaderia item in Mercaderia.Mercaderias where (im.ProductoID == item.ProductoID && (im!=item || im.ItemMercaderiaID!=item.ItemMercaderiaID || Mercaderia.Mercaderias.IndexOf(im)!=Mercaderia.Mercaderias.IndexOf(item))) select item).ToList();
                if (imsWithSameProduct != null && imsWithSameProduct.Count > 0 && Tipo!="Venta") {
                    state.AddModelError("Mercaderia.Mercaderias_" + Mercaderia.Mercaderias.IndexOf(im) + "__Descripcion", "No pueden agregarse los mismos artículos más de una vez a las mercaderias del cargamento");
                    return false;
                }
                ItemMercaderia itemModificado = (from cargamentoIM in Cargamento.Mercaderia.Mercaderias where cargamentoIM.ProductoID == im.ProductoID select cargamentoIM).SingleOrDefault();
                if (!eventoSumaMercaderia) { 
                    ItemMercaderia oldImChanged = (from ItemMercaderia oldIm in oldMercaderia.Mercaderias where oldIm.ProductoID == im.ProductoID && oldIm.ItemMercaderiaID == im.ItemMercaderiaID select oldIm).SingleOrDefault()??new ItemMercaderia();
                    int oldCantidadDelProducto = (from ItemMercaderia itemDeEvento in oldMercaderia.Mercaderias where im.ProductoID == itemDeEvento.ProductoID select itemDeEvento.Cantidad).Sum();
                    double oldPesoDelProducto = (from ItemMercaderia itemDeEvento in oldMercaderia.Mercaderias where im.ProductoID == itemDeEvento.ProductoID select itemDeEvento.Peso).Sum();                    
                    int cantidadDelProducto = (from ItemMercaderia itemDeEvento in Mercaderia.Mercaderias where im.ProductoID == itemDeEvento.ProductoID select itemDeEvento.Cantidad).Sum();
                    double pesoDelProducto = (from ItemMercaderia itemDeEvento in Mercaderia.Mercaderias where im.ProductoID == itemDeEvento.ProductoID select itemDeEvento.Peso).Sum();                    
                        
                    if (itemModificado != null ) {                        
                        if (itemModificado.Peso < (pesoDelProducto - oldPesoDelProducto)) {
                            state.AddModelError("Mercaderia.Mercaderias_" + Mercaderia.Mercaderias.IndexOf(im) + "__Peso", "El peso de los ítems en la mercadería no puede superar al peso de los ítems en la mercadería del cargamento");
                            return false;
                        }
                        if (itemModificado.Cantidad < (cantidadDelProducto-oldCantidadDelProducto)) {
                            state.AddModelError("Mercaderia.Mercaderias_" + Mercaderia.Mercaderias.IndexOf(im) + "__Cantidad", "Las cantidades de los ítems en la mercadería no puede superar a la cantidad de los ítems en la mercadería del cargamento");
                            return false;
                        }
                        //if (itemModificado.Bultos < (im.Bultos-oldImChanged.Bultos)) {
                        //    state.AddModelError("Mercaderia.Mercaderias_" + Mercaderia.Mercaderias.IndexOf(im) + "__Bultos", "Los bultos de los ítems en la mercadería no puede superar a los bultos de los ítems en la mercadería del cargamento");
                        //    return false;
                        //}
                    }
                    else {
                        if (oldImChanged!= null && (oldPesoDelProducto<pesoDelProducto || oldCantidadDelProducto<cantidadDelProducto)) { //  pueden sumarsele mercaderias borrando los eventos que restan mercaderia, incluso cuando ya no estan en los remanentes. 
                            state.AddModelError("Mercaderia.Mercaderias_" + Mercaderia.Mercaderias.IndexOf(im) + "__Descripcion", "No pueden incluirse ítems en la mercadería que no esten incluidos la mercadería del cargamento");
                            return false;
                        }
                    }
                    itemModificado = itemModificado ?? new ItemMercaderia();
                    oldImChanged = oldImChanged ?? new ItemMercaderia();
                    int oldCantidadDeVentasDelProducto = (from ItemMercaderia itemDeVenta in oldMercaderia.Mercaderias where im.ProductoID == itemDeVenta.ProductoID select itemDeVenta.Cantidad).Sum();
                    double oldPesoDeVentasDelProducto = (from ItemMercaderia itemDeVenta in oldMercaderia.Mercaderias where im.ProductoID == itemDeVenta.ProductoID select itemDeVenta.Peso).Sum();
                    int cantidadDeVentasDelProducto = (from ItemMercaderia itemDeVenta in Mercaderia.Mercaderias where im.ProductoID == itemDeVenta.ProductoID select itemDeVenta.Cantidad).Sum();
                    double pesoDeVentasDelProducto = (from ItemMercaderia itemDeVenta in Mercaderia.Mercaderias where im.ProductoID == itemDeVenta.ProductoID select itemDeVenta.Peso).Sum();
                    if (Cargamento.TipoVenta == "Precio Cerrado" && Tipo!="Decomisación"  && (itemModificado.Cantidad != cantidadDeVentasDelProducto-oldCantidadDeVentasDelProducto/*(im.Cantidad - oldImChanged.Cantidad)*/ || itemModificado.Peso != pesoDeVentasDelProducto-oldPesoDeVentasDelProducto/*(im.Peso - oldImChanged.Peso)*/ )) {
                        state.AddModelError("Mercaderia.Mercaderias_" + Mercaderia.Mercaderias.IndexOf(im) + "__Descripcion", "No se aceptan registros parciales de las mercaderías cuando la venta es a precio cerrado");
                        return false;
                    }
                }
            }
            return true;
        }

        private void updateEstado() {
            string estadoInicialValido=new EventoTipo(TipoEventoID).EstadoInicial;
            string estadoFinalValido=new EventoTipo(TipoEventoID).EstadoFinal;
            Cargamento.Estado = Cargamento.Estado == estadoInicialValido ? estadoFinalValido : Cargamento.Estado;
        }

        private void reverseEstado() {
            string estadoInicialValido=new EventoTipo(TipoEventoID).EstadoInicial;
            string estadoFinalValido=new EventoTipo(TipoEventoID).EstadoFinal;
            Cargamento.Estado = (Cargamento.Estado == estadoFinalValido || Cargamento.EnEstadoFinal)? estadoInicialValido : Cargamento.Estado;
        }

        private void updateMercaderia() {
            bool cargamentoVendido = true;
            bool eventoSumaMercaderia = new EventoTipo(this.TipoEventoID).SumaMercaderia;
            Mercaderia oldMercaderia = IsNew()?new Mercaderia():Mercaderia.SingleOrDefault(MercaderiaID??0)?? new Mercaderia();
            
            Cargamento.Ganancia += eventoSumaMercaderia ? 0 : Tipo=="Decomisación"? 0:Mercaderia.Precio-oldMercaderia.Precio;
            var mercaderiaCargamento = Cargamento.Mercaderia;
            if (eventoSumaMercaderia) {
                mercaderiaCargamento.Precio = Mercaderia.Precio;
                mercaderiaCargamento.Peso=  Mercaderia.Peso;
                mercaderiaCargamento.Bultos=  Mercaderia.Bultos;            
            }
            else {
                mercaderiaCargamento.Precio += -(Mercaderia.Precio-oldMercaderia.Precio);
                mercaderiaCargamento.Peso+=  -(Mercaderia.Peso-oldMercaderia.Peso);
                mercaderiaCargamento.Bultos+= -(Mercaderia.Bultos-oldMercaderia.Bultos); 
            }
            foreach(ItemMercaderia im in Mercaderia.Mercaderias){
                ItemMercaderia itemACambiar = (from ItemMercaderia cargamentoIm in mercaderiaCargamento.Mercaderias where cargamentoIm.ProductoID == im.ProductoID select cargamentoIm).SingleOrDefault();
                if (itemACambiar == null ) {
                    // es nuevo en el cargamento
                    // debo clonarlo para que las mercaderias referencien a distintos ItemMercaderias
                    ItemMercaderia oldImChanged = (from ItemMercaderia oldIm in oldMercaderia.Mercaderias where oldIm.ProductoID == im.ProductoID && oldIm.ItemMercaderiaID == im.ItemMercaderiaID select oldIm).SingleOrDefault();
                    
                    if (oldImChanged == null) {
                        // si no existia previamente, solo permito que altere las mercaderias del cargamento si es un evento que suma mercaderia, estilo checkpoint del estado del cargamento.
                        if (eventoSumaMercaderia) { 
                            mercaderiaCargamento.Mercaderias.Add(new ItemMercaderia() { ProductoID = im.ProductoID, Bultos = im.Bultos, Peso = im.Peso, PrecioUnitario = im.PrecioUnitario,PesoUnitario = im.PesoUnitario,  PrecioKg = im.PrecioKg, Cantidad = im.Cantidad, Precio = im.Precio });
                            // el cargamento no puede estar vendido en este caso porque se acaba de agregar un elemento
                            cargamentoVendido = false;
                        }
                    }
                    else {
                        // si el item existia en el evento pero no en el cargamento, acepto que sea una modificacion de eventos que restan mercaderia
                        if (!eventoSumaMercaderia) { 
                            mercaderiaCargamento.Mercaderias.Add(new ItemMercaderia() { ProductoID = im.ProductoID, Bultos = -(im.Bultos-oldImChanged.Bultos), Peso = -(im.Peso-oldImChanged.Peso), PrecioUnitario = oldImChanged.PrecioUnitario,PesoUnitario = oldImChanged.PesoUnitario,  PrecioKg = oldImChanged.PrecioKg, Cantidad = -(im.Cantidad-oldImChanged.Cantidad), Precio = -(im.Precio-oldImChanged.Precio)});
                            // el cargamento no puede estar vendido en este caso porque se acaba de agregar un elemento
                            cargamentoVendido = false;
                        }
                    }
                }
                else {
                    double nuevoPeso;
                    int nuevaCantidad;
                    ItemMercaderia oldImChanged = (from ItemMercaderia oldIm in oldMercaderia.Mercaderias where oldIm.ProductoID == im.ProductoID && oldIm.ItemMercaderiaID == im.ItemMercaderiaID select oldIm).SingleOrDefault();
                    
                    if (oldImChanged == null) {
                        // no existia el item en el evento.  
                        nuevoPeso = eventoSumaMercaderia ? im.Peso : -im.Peso;
                        nuevaCantidad = eventoSumaMercaderia ? im.Cantidad : -im.Cantidad;
                        itemACambiar.actualizarAgregaciones(eventoSumaMercaderia,new ItemMercaderia() {  Bultos = im.Bultos, Peso = nuevoPeso, PrecioUnitario = im.PrecioUnitario,PesoUnitario = im.PesoUnitario,  PrecioKg = im.PrecioKg, Cantidad = nuevaCantidad, Precio = im.Precio });
                    }
                    else {
                        if (!oldImChanged.Equals(im)) {
                            //el item existia pero cambio en alguna propiedad
                            nuevoPeso = eventoSumaMercaderia ? (im.Peso - oldImChanged.Peso):-(im.Peso - oldImChanged.Peso);
                            nuevaCantidad = eventoSumaMercaderia ? (im.Cantidad - oldImChanged.Cantidad) : -(im.Cantidad - oldImChanged.Cantidad);
                            itemACambiar.actualizarAgregaciones(eventoSumaMercaderia&&IsNew(),new ItemMercaderia() {  Bultos = im.Bultos, Peso = nuevoPeso, PrecioUnitario = im.PrecioUnitario,PesoUnitario = im.PesoUnitario, PrecioKg = im.PrecioKg, Cantidad = nuevaCantidad, Precio = im.Precio });
                        }
                    }
                    cargamentoVendido = itemACambiar.sinExistencias()&& cargamentoVendido;
                }
            }
            int cantidadRemanentes = (from ItemMercaderia im in Cargamento.Mercaderia.Mercaderias where !im.sinExistencias() select im).ToList().Count;
            if (cargamentoVendido || cantidadRemanentes == 0){
                if (!Cargamento.IsNew() && Cargamento.Estado=="Recibido" && cantidadRemanentes == 0)
                    Cargamento.Estado = "Vendido";
                }
            else{
                if (!eventoSumaMercaderia && cantidadRemanentes>0) {
                    // si el evento no suma mercaderia y no se vaciaron los items del cargamento entonces aun no esta vendido.
                    reverseEstado();
                }
            }
        }

        public void notifyIfObjectEnds() {
            if (Cargamento.EnEstadoFinal) {
                EventoTipo tipo = new EventoTipo(TipoEventoID);
                string url = "http://" + Sitio.WebsiteURL + "/Reporte/Liquidacion/"+ CargamentoID.ToString();
                string title = "[ProductosDeLaTierra] "+Cargamento.ToString()+" "+Cargamento.Estado;
                string message = Cargamento.ToString()+", enviado al cliente "+Cargamento.Cliente+", está "+Cargamento.Estado;
                foreach(Usuario user in searchNotificables(new ModelStateDictionary())){       
                    Notificacion.NotificarMasEmail( user,title, url,message,"Para ver la liquídacion del cargamento ingrese a: <a href=\"" + url + "\">"+ url + "</a>.");
                }
            }
        }

        public void notifyIfExistsDiffrence() {
            // solo se notifica si no es el evento inicial, el evento define la mercaderia del cargamento y existe una difecencia con las mercaderias actuales del mismo.
            if (new EventoTipo(TipoEventoID).SumaMercaderia && !Mercaderia.Equals(prevCheckpointEvent().Mercaderia) && TipoEventoID>1 ) {
                EventoTipo tipo = new EventoTipo(TipoEventoID);
                string url = "http://" + Sitio.WebsiteURL +"/Eventos/Evento/Editar/" + tipo.Nombre + "/" + CargamentoID.ToString(); // solo puede ir el cargamento id porque suma mercaderia y es unico para cada cargamento.
                string title = "[ProductosDeLaTierra] Diferencia de mercadería entre "+new Evento(){TipoEventoID=this.TipoEventoID-1}.Tipo+" y "+ Tipo +" de "+Cargamento.ToString();
                string message = "Se registró una diferencia entre las mercaderías en " + new Evento(){TipoEventoID=this.TipoEventoID-1}.Tipo + " y " + Tipo + " de " + Cargamento.ToString();
                foreach(Usuario user in searchNotificables(new ModelStateDictionary())){       
                    Notificacion.NotificarMasEmail( user,title, url,message,message+". Para ver las mercaderías ingrese a:<br/>"+"<br/><br/>"+new Evento(){TipoEventoID=this.TipoEventoID-1}.Tipo+": <a href=\"http://" + Sitio.WebsiteURL + "/" + "Eventos/Evento/Editar/" + new EventoTipo(TipoEventoID-1).Nombre + "/" + CargamentoID.ToString() + " \">http://" + Sitio.WebsiteURL + "/" + "Eventos/Evento/Editar/" + new EventoTipo(TipoEventoID-1).Nombre+ "/" + CargamentoID.ToString() + "</a>." + "<br/><br/>"+ Tipo +": <a href=\"" + url + "\">" +  url + "</a>.");
                }
            }
        }

        private List<Usuario> searchNotificables(ModelStateDictionary state) {
            var sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT Usuario.* FROM Usuario");
            sql.Append("INNER JOIN UsuarioRol ON UsuarioRol.UsuarioID = Usuario.UsuarioID");
            sql.Append("INNER JOIN Rol ON UsuarioRol.RolID = Rol.RolID ");
            sql.Append("WHERE (Rol.Nombre = 'Administrador' OR Usuario.ProveedorID = @0 ) ",Cargamento.ProveedorID);
            if (Sitio.EsEmpleado)
                sql.Append("AND Usuario.UsuarioID != @0",Sitio.Usuario.UsuarioID);
            List<Usuario> AdministradoresNotificables = DbHelper.CurrentDb().Fetch<Usuario>(sql);

            sql = PetaPoco.Sql.Builder;
            sql.Append("SELECT Usuario.* FROM Usuario");
            sql.Append("WHERE  Usuario.ProveedorID = @0  ", Cargamento.ProveedorID);
            List<Usuario> AdministradoresDeUsuariosNotificables = DbHelper.CurrentDb().Fetch<Usuario>(sql);

            AdministradoresNotificables.AddRange(AdministradoresDeUsuariosNotificables);
            EventoTipo tipo = new EventoTipo(TipoEventoID);
            Usuario usuarioANotificar= null;
            //                                                                                                         el cargador de cargamentos no deve ser notificado de nada
            if ( (AdministradoresDeUsuariosNotificables==null || AdministradoresDeUsuariosNotificables.Count == 0 ) && (tipo.RolNotificable == "Proveedor" || Cargamento.EnEstadoFinal)) {
                usuarioANotificar = Usuario.SingleOrDefault(this.Cargamento.ProveedorID);
            }
            if (tipo.RolNotificable == "Cliente") {
                usuarioANotificar = Usuario.SingleOrDefault(this.Cargamento.ClienteID);
            }
            if (usuarioANotificar ==null) {
                state.AddModelError("", "No se indicó un " + tipo.RolNotificable + " en el cargamento");
            }
            else {
                if (usuarioANotificar.Email.IsEmpty()){
                    state.AddModelError("", "No se indicó un email de " + tipo.RolNotificable + " para notificar");
                }
                else {
                    AdministradoresNotificables.Add(usuarioANotificar);
                }
            }
            return AdministradoresNotificables;
        }

        public void notify(ModelStateDictionary state) {                
            foreach(Usuario user in searchNotificables(state)){       
                notifyToUser( user,state);
            }
        }

        private void notifyToUser( Usuario usuarioANotificar, ModelStateDictionary state) {
            try {
                EventoTipo tipo = new EventoTipo(TipoEventoID);
                string url = "http://" + Sitio.WebsiteURL +"/Eventos/Evento/Editar/" + tipo.Nombre + "/" + (new EventoTipo(TipoEventoID).SumaMercaderia?CargamentoID.ToString():EventoID.ToString());
                string title = "[ProductosDeLaTierra] "+ Tipo +" "+Cargamento.ToString();
                string message = "El usuario " + Sitio.Usuario.Nombre + " registró " + Tipo + " de " + Cargamento.ToString();
                Notificacion.NotificarMasEmail(usuarioANotificar,title, url,message,message+ ". <br/><br/>Para verlo ingrese a <a href=\""+ url + "\">" + url + "</a> .");
            }
            catch {
                state.AddModelWarning("No fue posible notificar a " + usuarioANotificar.Nombre);
            }
        }

        private void deleteOldItems() {
            bool eventoSumaMercaderia = new EventoTipo(this.TipoEventoID).SumaMercaderia;
            var mercaderiaCargamento = Cargamento.Mercaderia;
            Mercaderia oldMercaderia = MercaderiaID.IsEmpty()?new Mercaderia():Mercaderia.SingleOrDefault(MercaderiaID??0)?? new Mercaderia();
            // elimino de las mercaderias del cargamentos los elementos que fueron removidos del evento.
                
            var sql = PetaPoco.Sql.Builder.Append("DELETE ItemMercaderia WHERE ItemMercaderiaID IN (");
            List<int> ImIDsActuales = (from ItemMercaderia Im in Mercaderia.Mercaderias select Im.ItemMercaderiaID).ToList();
            List<int> productIDsActuales = (from ItemMercaderia Im in Mercaderia.Mercaderias select Im.ProductoID).ToList();

            List<int> deletedImIDsEvent =  (from int id in (from ItemMercaderia oldIm in oldMercaderia.Mercaderias where !oldIm.ProductoID.IsEmpty() select oldIm.ItemMercaderiaID) where !ImIDsActuales.Contains(id) select id).ToList();
            List<ItemMercaderia> deletedItems = (from ItemMercaderia deletedIm in oldMercaderia.Mercaderias where deletedImIDsEvent.Contains(deletedIm.ItemMercaderiaID) select deletedIm).ToList();
            
            // si el evento suma mercaderia, entonces las mercaderias del cargamento son las actuales del evento y los items anteriores que no esten en la nueva mercaderia deben ser eliminados.
            if (eventoSumaMercaderia)
                deletedItems.AddRange((from ItemMercaderia deletedIm in mercaderiaCargamento.Mercaderias where !productIDsActuales.Contains( deletedIm.ProductoID) select deletedIm));
            
            // elimino tambien los items asociados al cargamento que se agotaron
            deletedItems.AddRange((from ItemMercaderia deletedIm in mercaderiaCargamento.Mercaderias where deletedIm.sinExistencias() select deletedIm));
            
            List<int> deletedImIDs = (from ItemMercaderia deletedIm in deletedItems select deletedIm.ItemMercaderiaID).ToList();
            foreach (ItemMercaderia deletedIm in deletedItems) {
                ItemMercaderia itemACambiar = (from ItemMercaderia cargamentoIm in mercaderiaCargamento.Mercaderias where cargamentoIm.ProductoID == deletedIm.ProductoID select cargamentoIm).SingleOrDefault();
                if (itemACambiar != null) {
                    // si el evento sumaba mercaderia y el elemento fue removido, entoces debe restarse las agregaciones
                    double nuevoPeso = eventoSumaMercaderia ? -deletedIm.Peso : deletedIm.Peso;
                    int nuevaCantidad = eventoSumaMercaderia ? -deletedIm.Cantidad : deletedIm.Cantidad;
                    itemACambiar.actualizarAgregaciones(false,new ItemMercaderia() {  Bultos = itemACambiar.Bultos, Peso = nuevoPeso, PrecioUnitario = itemACambiar.PrecioUnitario,PesoUnitario = itemACambiar.PesoUnitario,  PrecioKg = itemACambiar.PrecioKg, Cantidad = nuevaCantidad, Precio = itemACambiar.Precio });
                        
                    // Si se redujo las existencias del item a cero se lo debe sacar de las mercaderias del cargamento.
                    if (itemACambiar.sinExistencias() && eventoSumaMercaderia) {
                        deletedImIDs.Add(itemACambiar.ItemMercaderiaID);
                        Cargamento.Mercaderia.Mercaderias.Remove(itemACambiar);
                    }                        
                }
                else {
                    if (!eventoSumaMercaderia) {
                        // se elimino un elemento del evento que resta mercaderia, por lo que debe restituirse en el cargamento.
                        mercaderiaCargamento.Mercaderias.Add(new ItemMercaderia() { ProductoID = deletedIm.ProductoID, Bultos = deletedIm.Bultos, Peso = deletedIm.Peso, PrecioUnitario = deletedIm.PrecioUnitario,PesoUnitario = deletedIm.PesoUnitario,  PrecioKg = deletedIm.PrecioKg, Cantidad = deletedIm.Cantidad, Precio = deletedIm.Precio});
                        reverseEstado();
                    }
                }
            }
                
            // solo elimino el item de las mercaderias del evento si se borro (su productId esta vacio), lo dejo si solamente cambio el productID
            List <int>currentImIDs = (from ItemMercaderia Im in Mercaderia.Mercaderias where !Im.ProductoID.IsEmpty() select Im.ItemMercaderiaID).ToList();
            deletedImIDs = (from int id in deletedImIDs where !currentImIDs.Contains(id) select id).ToList();                
            for (int i =0 ; i<deletedImIDs.Count-1;++i){
                sql.Append(deletedImIDs[i].ToString() + ",");
            }
            if (deletedImIDs.Count > 0) { 
                sql.Append(deletedImIDs[deletedImIDs.Count-1].ToString()+")");
                DbHelper.CurrentDb().Execute(sql);
            }
            // siempre que la mercaderia vieja y la nueva no coincidan deberá actualizarse la mercaderia del cargamento porque la misma cambió
                
            // si se quiere no guardar el evento podria solo guardarse su mercaderia
            //mercaderiaCargamento.DoSave();
            //Cargamento.MercaderiaID = mercaderiaCargamento.MercaderiaID;
        }

        private void updateRelatedObjects(){
            updateEstado();
            
            Mercaderia oldMercaderia = IsNew()?new Mercaderia():Mercaderia.SingleOrDefault(MercaderiaID??0)?? new Mercaderia();
            if (!oldMercaderia.Equals(Mercaderia)) {
                try {
                    notifyIfExistsDiffrence();
                } catch(Exception){

                }
                
                updateMercaderia();
                deleteOldItems();
                
                try {
                    notifyIfObjectEnds();
                } catch(Exception){

                }
            }
        }

        public void Delete() {
            var db = DbHelper.CurrentDb();
            using (var scope = db.GetTransaction()) {
                DoDelete();
			    // permito que las clases derivadas modifiquen lo que necesiten.
                AfterDelete();
                scope.Complete();
            } 
        }

        public void DoDelete() {
            var db = DbHelper.CurrentDb();
            // desvinculo los objetos relacionados al evento
            BeforeDelete();

            db.Execute("DELETE FROM Evento WHERE EventoID = @0", this.EventoID);
        }

        // no puede modificarse si se registraron eventos posteriores a este, ya que podria dejarlos en un estado inválido.
        public virtual bool CanUpdate(ModelStateDictionary state) {
            if (IsNew() && Cargamento.Estado!= new EventoTipo(TipoEventoID).EstadoInicial){
                state.AddModelError("CargamentoID", "El evento " + new EventoTipo(TipoEventoID).Nombre + " no puede registrarse sobre un cargamento " + Cargamento.Estado);
                return false;
            }
            // permito que halla eventos en el mismo dia solo si estoy editando el de mayor indice. Para el caso de la decomisacion, permito que tambien se permitan ventas el mismo dia.
            var sql = BaseQuery().Append("WHERE CargamentoID = @0 AND ( Fecha > @1 OR (Fecha = @1 AND TipoEventoID >= @2) ", CargamentoID, Fecha, TipoEventoID==new EventoTipo("Venta").ID? ++new EventoTipo("Decomisacion").ID:TipoEventoID,EventoID);
            sql.Append(TipoEventoID==new EventoTipo("Venta").ID?" OR (Fecha = @0 AND TipoEventoID= @1 ) )" :")",Fecha,TipoEventoID);
            sql.Append("AND EventoID <> @0",EventoID);
            List<Evento> eventosPosteriores = DbHelper.CurrentDb().Fetch<Evento>(sql).ToList();
            if( eventosPosteriores.Count > 0){
                state.AddModelError("CargamentoID", "El cargamento posee eventos ya registrados en conflicto por ser del mismo tipo en la fecha actual ó posteriores a la fecha actual");
                return false;
            }
            return true;
        }

        public virtual bool CanDelete(ModelStateDictionary state) {
            return CanUpdate(state);
        }

        // se tomaran acciones para dejar la base de datos en un estado válido y eliminar el objeto .
        public virtual void BeforeDelete() {
            if (MercaderiaID > 0){
                foreach (ItemMercaderia im in Mercaderia.Mercaderias) {
                    im.Peso = im.Cantidad = im.Bultos = 0;
                }
                Mercaderia.Peso = Mercaderia.Precio = Mercaderia.Bultos = 0;
                if (new EventoTipo(TipoEventoID).SumaMercaderia) {
                    if ((TipoEventoID-1) == 0) {
                        // se elimina el evento que dio origen al cargamento.
                        // si este evento dio origen, se encarga de borrar todo.
                        DbHelper.CurrentDb().Execute("DELETE FROM Evento WHERE EventoID = @0", this.EventoID);
                        Cargamento.DoDelete();
                    }
                    else {
                        reverseEstado();
                        Evento eventToRestore = prevCheckpointEvent();
                        eventToRestore.EventoID = 0;
                        eventToRestore.Cargamento = Cargamento;
                        eventToRestore.updateRelatedObjects();
                        Cargamento.DoSave();
                    }
                }
                else {
                    updateMercaderia();
                    reverseEstado();
                    Cargamento.DoSave();
                }
            }
        }

        public Evento prevCheckpointEvent() {
            EventoTipo type = new EventoTipo(TipoEventoID);
            while (!type.SumaMercaderia)
                type = new EventoTipo(type.ID-1);
            type = new EventoTipo(type.ID-1);
            return Evento.SingleOrDefault(type.ID, CargamentoID??0);
        }

        // solo se tomaran acciones para eliminar los objetos que alla creado.
        public virtual void AfterDelete() {
            Mercaderia.DoDelete();
        }

        public bool mostrarRemanente(){
            return this.IsNew() && ! new EventoTipo(this.TipoEventoID).SumaMercaderia ;
        }

        public bool IsNew() {
            return EventoID.IsEmpty();
        }
        /*
        public bool IsValidCargamento(System.Web.Mvc.ModelStateDictionary ModelState) {
            // Si existe en DB( y se tiene su ID ) ya es válido, sino se corrobora que sea válido.
            if (CargamentoID.IsEmpty() && !Cargamento.IsValid(ModelState)){
                // Cargamento.IsValid llena el ModelState con lo errores correspondientes.
                return false;
            }
            return true;
        }

        public bool IsValidMercaderia(System.Web.Mvc.ModelStateDictionary ModelState) {
            // Si existe en DB( y se tiene su ID ) ya es válido, sino se corrobora que sea válido.
            if (MercaderiaID.IsEmpty() && !Mercaderia.IsValid(ModelState)){
                // Mercaderia.IsValid llena el ModelState con lo errores correspondientes.
                return false;
            }
            return true;
        }
        */

    }
}