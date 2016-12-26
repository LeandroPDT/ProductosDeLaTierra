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
    [TableName("ItemCobranza")]
    [PrimaryKey("ItemCobranzaID")]
    [ExplicitColumns]
    public class ItemCobranza {

        [PetaPoco.Column("ItemCobranzaID")]
        [Display(Name="ID")]
        public int ItemCobranzaID { get; set; }

        [PetaPoco.Column("Monto")]
        [Display(Name="Monto ($)")]
        [Range(0,10000000)]
        public double Monto { get; set; }

        [PetaPoco.Column("Referencia")]
        [Display(Name = "Código Referencia Ítem")]
        public String Referencia { get; set; }

        [PetaPoco.Column("Medio")]
        [Display(Name = "Medio de pago")]
        public String Medio { get; set; }

        [PetaPoco.Column("CobranzaID")]
        public int CobranzaID { get; set; }

        [PetaPoco.Column("CargamentoID")]
        [Display(Name = "Cargamento")]
        public int CargamentoID { get; set; }

        [ResultColumn]
        public int NumeroRemito { get; set; }

        [PetaPoco.Column("EventoID")]
        [Display(Name = "Evento")]
        public int? EventoID { get; set; }

        [ResultColumn]
        public int ProveedorID{ get; set; }

        private Cargamento _Cargamento;
        public Cargamento Cargamento
        {
            get
            {
                if (_Cargamento == null)
                {
                    _Cargamento = CargamentoID.IsEmpty() ? new Cargamento() : Cargamento.SingleOrDefault(CargamentoID) ?? new Cargamento();
                }
                return _Cargamento;
            }
            set
            {
                _Cargamento = value;
            }
        }

        // virtual proxy del evento
        private Evento _Evento;
        public Evento Evento {
            get {
                var Tipo = new EventoTipo("Cobro");
                if (_Evento == null) {
                    _Evento = EventoID.IsEmpty() ? new Evento() { Ganancia = Monto, TipoEventoID = Tipo.ID, CargamentoID = CargamentoID, Cargamento = Cargamento, Mercaderia = null } : Evento.SingleOrDefault(EventoID??0) ;
                    if (_Evento == null) {
                        _Evento = new Evento() { Ganancia = Monto, TipoEventoID = Tipo.ID, CargamentoID = CargamentoID, Cargamento = Cargamento, Mercaderia = null };
                    }
                }
                return _Evento;
            }
            set {
                _Evento = value;
            }
        }

        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
            sql.Append("ItemCobranza.*, Cargamento.NumeroRemito as NumeroRemito");
            sql.Append("FROM ItemCobranza");
            sql.Append("INNER JOIN Cargamento ON Cargamento.CargamentoID = ItemCobranza.CargamentoID");
            return sql;
        }
        
        public bool IsValid(ModelStateDictionary state) {
            if (Monto <= 0) {
                state.AddModelError("", "Los montos de la cobranza deben ser positivos");
                return false;
            }
            if (!IsValid() || !Cargamento.IsValid(state) || !Evento.IsValid(state)) {
                state.AddModelError("","Al menos un item de la cobranza es inválido");
                return false;
            }       
            return true;
        }

        public bool Equals(ItemCobranza other) {
            return this.CargamentoID == other.CargamentoID && this.Monto == other.Monto && this.Referencia == other.Referencia;
        }

        private bool IsValid() {
            if (IsNew())
                return Cargamento.Estado == "Vendido";
            else
                return Cargamento.Estado == "Vendido" || Cargamento.Estado == "Cobrado";
        }

        private bool IsNew() {
            return ItemCobranzaID.IsEmpty();
        }



        // únicamente guarda el estado, usable en transacciones mas grandes.
        public virtual void DoSave() {
            BeforeSave();
            DbHelper.CurrentDb().SaveAndLog(this);
        }

        public virtual void BeforeSave() {
            Evento.Save();
            EventoID = Evento.EventoID;
        }

        public void DoDelete() {
            var db = DbHelper.CurrentDb();
            db.Execute("DELETE FROM ItemCobranza WHERE ItemCobranzaID = @0", this.ItemCobranzaID);
            AfterDelete();
        }

        public void AfterDelete() {
            Evento.DoDelete();
            Evento.AfterDelete();
        }
        
        public static SelectList MedioSelectList(String ValorActualSeleccionado) {
            var retval = new List<KeyValuePair<String, String>>();
            retval.Add(new KeyValuePair<String, String>("", ""));
            retval.Add(new KeyValuePair<String, String>("Cheque", "Cheque"));
            retval.Add(new KeyValuePair<String, String>("Depósito", "Depósito"));
            retval.Add(new KeyValuePair<String, String>("Efectivo", "Efectivo"));
            retval.Add(new KeyValuePair<String, String>("Transferencia", "Transferencia"));
            return new SelectList(retval, "Key", "Value", ValorActualSeleccionado);
        }

    }
}