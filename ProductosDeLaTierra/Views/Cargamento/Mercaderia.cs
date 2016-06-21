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
    [TableName("Mercaderia")]
    [PrimaryKey("MercaderiaID")]
    [ExplicitColumns]
    public class Mercaderia {

        [PetaPoco.Column("MercaderiaID")]
        [Display(Name="ID")]
        public int MercaderiaID { get; set; }
        
        [PetaPoco.Column("Peso")]
        [Display(Name="Peso (Kg)")]
        [Range(0,100000)]
        public double Peso{ get; set; }

        [PetaPoco.Column("Precio")]
        [Display(Name="Precio ($)")]
        [Range(0,100000)]
        public double Precio{ get; set; }

        [PetaPoco.Column("Bultos")]
        [Display(Name="Bultos")]
        [Range(0,100000)]
        public int Bultos { get; set; }
        
        private List<ItemMercaderia> _Mercaderias;
        public List<ItemMercaderia> Mercaderias {
            get {
                if ((_Mercaderias == null || _Mercaderias.Count == 0)&& !MercaderiaID.IsEmpty()) {
                    var sql = ItemMercaderia.BaseQuery();
                    sql.Append("Where MercaderiaID = @0", MercaderiaID);
                    _Mercaderias = DbHelper.CurrentDb().Fetch<ItemMercaderia>(sql).ToList();
                }
                if (MercaderiaID.IsEmpty() && !HasItems()) {
                    _Mercaderias = new List<ItemMercaderia>();
                    _Mercaderias.Add(new ItemMercaderia());
                }
                return _Mercaderias;
            }
            set {
                _Mercaderias = value;

            }
        }
        
        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
            sql.Append("Mercaderia.*");
            sql.Append("FROM Mercaderia");
            return sql;
        }

        public static Mercaderia SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE MercaderiaID = @0",id);
            return DbHelper.CurrentDb().SingleOrDefault<Mercaderia>(sql);
        }

        public bool IsValid(ModelStateDictionary state) {
            if (!HasItems()) {
                state.AddModelError("MercaderiaID", "Deben especificarse las mercaderias y sus existencias");
                return false;
            }
            return true;
        }

        public void DoDelete() {
            DbHelper.CurrentDb().Execute("DELETE FROM ItemMercaderia WHERE MercaderiaID = @0",MercaderiaID);
            DbHelper.CurrentDb().Execute("DELETE FROM Mercaderia WHERE MercaderiaID = @0",MercaderiaID);
        }

        
        // únicamente guarda el estado evitando crear una transacción, usable en transacciones mas grandes.
        public virtual void DoSave() {
            //BeforeSave();
            DbHelper.CurrentDb().SaveAndLog(this);
            AfterSave();
        }

        // elimina ItemMercaderia/s vacios
        private void validateMercaderias() {
            _Mercaderias = (from ItemMercaderia im in _Mercaderias where (!im.ProductoID.IsEmpty()  && !im.sinExistencias()) select im).ToList();
        }

        public virtual void AfterSave() {
            if (HasItems()) {
                foreach (ItemMercaderia im in _Mercaderias) {
                    im.MercaderiaID = MercaderiaID;
                    DbHelper.CurrentDb().SaveAndLog(im);
                }   
            }
        }  

        public bool Equals(Mercaderia other) {
            validateMercaderias();
            other.validateMercaderias();
            if (other.Mercaderias.Count == this.Mercaderias.Count && Peso == other.Peso && Precio==other.Precio && Bultos==other.Bultos) {                
                foreach (ItemMercaderia im in this.Mercaderias) {
                    if ((from ItemMercaderia otherIm in other.Mercaderias where otherIm.ProductoID==im.ProductoID && (im.Peso != otherIm.Peso || im.Cantidad != otherIm.Cantidad ||im.Bultos != otherIm.Bultos) select im).ToList().Count >0 )
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        public bool HasItems() {
            validateMercaderias();
            return !(_Mercaderias == null || _Mercaderias.Count == 0);
        }

        public Mercaderia() {
            MercaderiaID = 0;
            Mercaderias = new List<ItemMercaderia>();
        }

        public Mercaderia clone() {
            var newMercaderia = new Mercaderia();
            newMercaderia.Peso = (from ItemMercaderia im in Mercaderias select im.Peso).Sum();
            newMercaderia.Precio = (from ItemMercaderia im in Mercaderias select im.Precio).Sum();
            //newMercaderia.Bultos = (from ItemMercaderia im in Mercaderias select im.Bultos).Sum();
            foreach (ItemMercaderia im in this.Mercaderias) {
                newMercaderia.Mercaderias.Add(im.clone());
            }
            return newMercaderia;
        }

    }
}