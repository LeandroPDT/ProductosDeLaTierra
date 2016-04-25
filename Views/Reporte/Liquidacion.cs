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
using BizLibMVC;



namespace Site.Models {
    public class Liquidacion{               
        
        public String NombreCargamento { get; set; }
        
        [PetaPoco.Column("NumeroRemito")]
        [Display(Name="Nro. Remito")]
        public int NumeroRemito { get;set;}
        
		[PetaPoco.Column("FechaEnvio")]
		[Display(Name = "Fecha de Envío")]
		[DataType(DataType.DateTime)]
		public DateTime FechaEnvio { get; set; }

        [Display(Name="Proveedor")]
        public String Proveedor { get; set; }

        [Display(Name="Cliente")]
        public String Cliente { get; set; }
        
        [Display(Name="Ganancia")]
        public Double Ganancia  { get; set; }
        
        [Display(Name="Comision cobrada")]
        public Double ComisionCobrada  { get; set; }
        
        [PetaPoco.Column("CostoFlete")]
        [Display(Name="Costo del flete ($)")]
        public Double CostoFlete{ get; set; }

        [Display(Name="Costo de la descarga ($)")]
        public Double CostoDescarga{ get; set; }
        
        [Display(Name="Ganancia Total")]
        public Double GananciaTotal  { get; set; }

        public List<LiquidacionItem> ItemsVendidos { get; set;}

        public Liquidacion(int CargamentoID) {
            var Cargamento = Models.Cargamento.SingleOrDefault(CargamentoID);
            if (Cargamento != null) {
                NombreCargamento = Cargamento.ToString();
                NumeroRemito = Cargamento.NumeroRemito;
                FechaEnvio = Cargamento.FechaEnvio;
                Proveedor = Cargamento.Proveedor;
                Cliente = Cargamento.Cliente;

                var sql = PetaPoco.Sql.Builder;
                sql.Append("SELECT ");
                sql.Append("Venta.Fecha as Fecha,");
                sql.Append("ISNULL(Convert(varchar(12),Producto.CodigoArticulo ),'') + ' - ' + Convert(varchar(50),Producto.Descripcion) as Articulo,");
                sql.Append("ItemVenta.Cantidad as Cantidad,");
                sql.Append("ItemVenta.PrecioUnitario as Precio,");
                sql.Append("ItemVenta.Precio as Total");
                sql.Append("FROM Evento Venta");
                sql.Append("INNER JOIN ItemMercaderia ItemVenta on ItemVenta.MercaderiaID = Venta.MercaderiaID");
                sql.Append("INNER JOIN Producto on  ItemVenta.ProductoID = Producto.ProductoID");
                sql.Append("WHERE Venta.CargamentoID = @0 AND Venta.TipoEventoID = @1", Cargamento.CargamentoID,new EventoTipo("Venta").ID);            
                sql.Append("ORDER BY Venta.Fecha DESC");
                ItemsVendidos = DbHelper.CurrentDb().Fetch<LiquidacionItem>(sql);
                Ganancia = (from LiquidacionItem imVendido in ItemsVendidos select imVendido.Total).Sum();
                var usuarioProveedor = Usuario.SingleOrDefault(Cargamento.ProveedorID)??new Usuario();
                var porcComision = usuarioProveedor.Comision/100;

                ComisionCobrada = porcComision * Ganancia;
                CostoDescarga = Cargamento.CostoDescarga;
                CostoFlete = Cargamento.CostoFlete;

                GananciaTotal = Ganancia - ComisionCobrada - CostoDescarga - CostoFlete;
            }
        }

        public override string ToString() {
            return "Liquidación Remito Nro. " + NumeroRemito.ToString();
        }

        public class LiquidacionItem {
            
            public int NumeroRemito { get; set; }

            public DateTime Fecha { get; set; }

            public String Articulo { get; set; }

            public int Cantidad { get; set; }
            
		    [DataType(DataType.Currency)]
            public Double Precio { get; set; }
            
		    [DataType(DataType.Currency)]
            public Double Total { get; set; }
            
        }

    }
}