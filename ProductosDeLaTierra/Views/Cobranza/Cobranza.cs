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
    [TableName("Cobranza")]
    [PrimaryKey("CobranzaID")]
    [ExplicitColumns]
    public class Cobranza {

        [PetaPoco.Column("CobranzaID")]
        [Display(Name="ID")]
        public int CobranzaID { get; set; }

        [PetaPoco.Column("Fecha")]
        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime Fecha { get; set; }

        [PetaPoco.Column("Referencia")]
        [Display(Name="Código de Referencia")]
        public String Referencia { get; set; }

        [PetaPoco.Column("ProveedorID")]
        [Display(Name = "Proveedor")]
        [Required]
        public int? ProveedorID { get; set; }

        [ResultColumn]
        [Display(Name = "Proveedor")]
        public String Proveedor { get; set; }

        [PetaPoco.Column("Monto")]
        [Display(Name = "Monto ($)")]
        [Range(0, 10000000)]
        [Required]
        public double Monto { get; set; }

        [PetaPoco.Column("Notas")]
		[Display(Name = "Notas")]
		[DataType(DataType.MultilineText)]
		public String Notas{ get; set; }

        private List<ItemCobranza> _Cobranzas;
        public List<ItemCobranza> Cobranzas {
            get {
                if ((_Cobranzas == null || _Cobranzas.Count == 0) && !CobranzaID.IsEmpty()) {
                    var sql = ItemCobranza.BaseQuery();
                    sql.Append("Where CobranzaID = @0", CobranzaID);
                    _Cobranzas = DbHelper.CurrentDb().Fetch<ItemCobranza>(sql).ToList();
                }
                if ( CobranzaID.IsEmpty() && ((_Cobranzas == null || _Cobranzas.Count == 0) || !HasItems())) {
                    _Cobranzas = new List<ItemCobranza>();
                    _Cobranzas.Add(new ItemCobranza() { Evento=new Evento() { TipoEventoID=new EventoTipo("Cobro").ID} });
                }
                return _Cobranzas;
            }
            set
            {
                _Cobranzas = value;
            }
        }

        public Cobranza() {
            Fecha = DateTime.Now;
        }

        public bool HasItems()
        {
            validateCobranzas();
            return !(_Cobranzas == null || _Cobranzas.Count == 0);
        }

        // no puede modificarse si se registraron eventos posteriores a este, ya que podria dejarlos en un estado inválido.
        public virtual bool CanUpdate(ModelStateDictionary state) {
            if (HasItems()) {
                foreach (ItemCobranza ic in _Cobranzas) {
                    if (!ic.Evento.CanUpdate(state))
                        return false;
                }
            }
            return true;
        }

        public void notify(ModelStateDictionary modelState) {
            foreach (ItemCobranza ic in _Cobranzas) {
                ic.Evento.notify(modelState);              
            }
        }

        // elimina ItemCobranza/s vacios
        private void validateCobranzas()
        {   
            if (_Cobranzas!=null)
                _Cobranzas = (from ItemCobranza im in _Cobranzas where  im.IsValid(new ModelStateDictionary()) select im).ToList();
        }

        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
            sql.Append("Cobranza.*, usuario.Nombre As Proveedor");
            sql.Append("FROM Cobranza");
            sql.Append("INNER JOIN Usuario usuario ON Cobranza.ProveedorID=usuario.UsuarioID");
            return sql;
        }

        public static Cobranza SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE CobranzaID = @0",id);
            return DbHelper.CurrentDb().SingleOrDefault<Cobranza>(sql);
        }


        public override string ToString() {
            return !Referencia.IsEmpty()? "Cobranza " + Referencia:"Nueva Cobranza";
        }

        public bool IsValid(ModelStateDictionary state) {
            if (Referencia.IsEmpty()) {
                state.AddModelError("Referencia", "Debe especificarse el código de referencia");
                return false;
            }
            if (Sitio.EsEmpleado) { 
                if ( ProveedorID.IsEmpty() ) {
                    state.AddModelError("ProveedorID", "Debe especificarse el proveedor del Cobranza");
                    return false;
                }
            }
            if (!Usuario.HasRol(ProveedorID??0,"Proveedor")) {
                state.AddModelError("ProveedorID", "Debe especificarse un proveedor válido");
                return false;
            }
            else {
                if (ProveedorID != Sitio.Usuario.UsuarioID && !Sitio.EsEmpleado && ProveedorID != Sitio.Usuario.ProveedorID) {
                    state.AddModelError("ProveedorID", "No pueden registrarse cobranzas de otros usuarios");
                    return false;
                }
            }
            if (HasItems()) {
                foreach (ItemCobranza ic in Cobranzas) {
                    if (!ic.IsValid(state))
                        return false;
                }
            } else {
                state.AddModelError("", "Deben especificarse ítems válidos para la cobranza");
                return false;
            }
            return true;
        }

        // únicamente guarda el estado evitando crear una transacción, usable en transacciones mas grandes.
        public virtual void DoSave() {
            BeforeSave();
            DbHelper.CurrentDb().SaveAndLog(this);
            AfterSave();
        }

        public virtual void BeforeSave() {
            deleteOldItems();
        }

        public virtual void AfterSave() {
            if (HasItems()) {
                foreach (ItemCobranza ic in _Cobranzas) {
                    ic.CobranzaID = CobranzaID;
                    ic.Evento.Fecha = Fecha;
                    ic.Evento.Ganancia = Monto;
                    ic.DoSave();
                }
            }
        }


        public void DoDelete() {
            Cobranzas = Cobranzas;
            DbHelper.CurrentDb().Execute("DELETE FROM ItemCobranza WHERE CobranzaID = @0", CobranzaID);
            DbHelper.CurrentDb().Execute("DELETE FROM Cobranza WHERE CobranzaID = @0", CobranzaID);
        }
        public void Delete() {
            DoDelete();
            if (HasItems()) {
                foreach (ItemCobranza ic in _Cobranzas) {
                    ic.Evento.Delete();
                }
            }
        }

        public bool HasUser(int UsuarioID) {
            return (this.ProveedorID == UsuarioID );
        }

        public bool IsNew(){
            return CobranzaID.IsEmpty();
        }
                
        // podrá gestionar si tiene el permiso y, o es empleado o participa del evento (con el rol que puede editarlo).
        public bool CurrentUserCanAccessToFunction(Seguridad.Feature function) {
            return (Seguridad.CanAccessToFunction(Sitio.Usuario.UsuarioID,(int)Seguridad.Permisos.EventoCobro,function) && (Sitio.EsEmpleado || HasUser(Sitio.Usuario.UsuarioID) || HasUser(Sitio.Usuario.ProveedorID??0)));
        }
        
        private void deleteOldItems() {
            foreach (ItemCobranza oldIc in IsNew()? new List<ItemCobranza>(): Cobranza.SingleOrDefault(CobranzaID).Cobranzas) {
                ItemCobranza sameIcInThisCobranzaInstance = (from ItemCobranza ic in Cobranzas where ic.Referencia == oldIc.Referencia && ic.CargamentoID == oldIc.CargamentoID && ic.ItemCobranzaID == oldIc.ItemCobranzaID select ic).SingleOrDefault();
                if (sameIcInThisCobranzaInstance == null) {
                    oldIc.DoDelete();
                }
            }
        }

    }
}