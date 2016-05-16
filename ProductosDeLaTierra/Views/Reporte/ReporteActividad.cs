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
    public class ReporteActividad{              
                
        public string Cargamento { get; set; }
        public string Producto { get; set; }
        public int CantidadPorPagina { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int? ProveedorID { get; set; }
        public String Proveedor{ get; set; }
        public int? ClienteID { get; set; }
        public String Cliente{ get; set; }
        public int? ProductoID { get; set; }

        public List<ReporteActividadItem> Resultado { get; set;}

        public ReporteActividad() {
            Cargamento = Sitio.GetPref("ReporteActividad-Cargamento", "");
            Producto = Sitio.GetPref("ReporteActividad-Producto", "");
            CantidadPorPagina = Sitio.GetPref("ReporteActividad-CantidadPorPagina", 50);
            FechaDesde = Sitio.GetPref("ReporteActividad-FechaDesde", new DateTime(DateTime.Today.Year, 1, 1));//DateTime.Today.FirstDayOfMonth()
            FechaHasta = Sitio.GetPref("ReporteActividad-FechaHasta", DateTime.Today);
            ProveedorID = Sitio.EsEmpleado || Usuario.HasRol(Sitio.Usuario.UsuarioID,"Cliente")? Sitio.GetPref("ReporteActividad-ProveedorID", "").AsInt().ZeroToNull():Sitio.Usuario.ProveedorID.IsEmpty()?Sitio.Usuario.UsuarioID:Sitio.Usuario.ProveedorID;
            Proveedor = Sitio.GetPref("ReporteActividad-Proveedor", "");
            ClienteID =  Sitio.EsEmpleado || Usuario.HasRol(Sitio.Usuario.UsuarioID,"Proveedor")? Sitio.GetPref("ReporteActividad-ClienteID", "").AsInt().ZeroToNull():Sitio.Usuario.ProveedorID.IsEmpty()?Sitio.Usuario.UsuarioID:0;
            Cliente = Sitio.GetPref("ReporteActividad-Cliente", "");
        }

        public void SetPref() {
            Sitio.SetPref("ReporteActividad-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("ReporteActividad-Cargamento", Cargamento);
            Sitio.SetPref("ReporteActividad-Producto", Producto);
            Sitio.SetPref("ReporteActividad-FechaDesde", FechaDesde);
            Sitio.SetPref("ReporteActividad-FechaHasta", FechaHasta);
            Sitio.SetPref("ReporteActividad-ProveedorID", ProveedorID??0);
            Sitio.SetPref("ReporteActividad-Proveedor", Proveedor);
            Sitio.SetPref("ReporteActividad-ClienteID", ClienteID??0);
            Sitio.SetPref("ReporteActividad-Cliente", Cliente);
        }

        public void CalcResultado() {
            ProveedorID = ProveedorID.ZeroToNull();
            ClienteID = ClienteID.ZeroToNull();
            var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(this.CantidadPorPagina);
            sql.Append("Cargamento.NumeroRemito as NumeroRemito, ");
            sql.Append("Cargamento.FechaEnvio as FechaEnvio,");
            sql.Append("Proveedor.Nombre as Proveedor,");
            sql.Append("Cliente.Nombre as Cliente,");
            sql.Append("ISNULL(Convert(varchar(12),Producto.CodigoArticulo ),'') + ' - ' + Convert(varchar(50),Producto.Descripcion) as Articulo,");
            sql.Append("ItemEnvio.Cantidad as Enviado,");
            sql.Append("ItemRecepcion.Cantidad as Recibido,");
            sql.Append("ISNULL(Sum(ItemVenta.Cantidad)/ ISNULL(NULLIF(Count(distinct Decomisacion.EventoID),0),1) ,0)as Vendido,");
            sql.Append("ISNULL(Sum(ItemDecomisacion.Cantidad)/ ISNULL(NULLIF(Count(distinct ItemVenta.ItemMercaderiaID),0),1),0) as Decomisado,");
            sql.Append("ISNULL(Sum(ItemVenta.Precio) / ISNULL(NULLIF(Count(distinct Decomisacion.EventoID), 0), 1), 0) / NULLIF(Sum(ItemVenta.Cantidad) / ISNULL(NULLIF(Count(distinct Decomisacion.EventoID), 0), 1), 0) as PrecioUnitarioPromedio,");
            sql.Append("ISNULL(Sum(ItemVenta.Precio )/ISNULL(NULLIF(Count(distinct Decomisacion.EventoID),0),1),0 ) as Total,");
            sql.Append("ItemRecepcion.Cantidad-( ISNULL(Sum(ItemVenta.Cantidad)/ ISNULL(NULLIF(Count(distinct Decomisacion.EventoID),0),1) ,0) + ISNULL(Sum(ItemDecomisacion.Cantidad)/ ISNULL(NULLIF(Count(distinct ItemVenta.ItemMercaderiaID),0),1),0) ) as Remanente,");
            sql.Append("(CASE WHEN Envio.Notas IS NULL	THEN '' ELSE ('Envío' + ' del ' + CONVERT(VARCHAR(10), Envio.Fecha, 103) + ' :\n'+ CAST(Envio.Notas as NVARCHAR(MAX)) +'\n\n') END)+(CASE WHEN Recepcion.Notas IS NULL  THEN '' ELSE('Recepcíon' + ' del ' + CONVERT(VARCHAR(10), Recepcion.Fecha, 103) + ' :\n' + CAST(Recepcion.Notas as NVARCHAR(MAX)) + '\n\n') END) + (CASE WHEN Venta.Notas IS NULL  THEN '' ELSE('Venta' + ' del ' + CONVERT(VARCHAR(10), Venta.Fecha, 103) + ' :\n' + CAST(Venta.Notas as NVARCHAR(MAX)) + '\n\n') END) +(CASE WHEN Decomisacion.Notas IS NULL THEN '' ELSE('Decomisacíon' + ' del ' + CONVERT(VARCHAR(10), Decomisacion.Fecha, 103) + ' :\n' + CAST(Decomisacion.Notas as NVARCHAR(MAX)) + '\n\n') END) as Observaciones,");
            sql.Append("(CASE WHEN EXISTS (SELECT * FROM ArchivoAdjunto WHERE ArchivoAdjunto.Entidad = 'Cargamento' AND ArchivoAdjunto.ID = Cargamento.CargamentoID)  THEN 1 ELSE 0 END) as TieneArchivosAdjuntos,");
            sql.Append("Cargamento.CargamentoID as CargamentoID,");
            sql.Append("Producto.ProductoID as ProductoID");
            sql.Append("FROM Cargamento");            
            sql.Append("INNER JOIN Usuario Proveedor on Proveedor.UsuarioID = Cargamento.ProveedorID");
            sql.Append("INNER JOIN Usuario Cliente on Cliente.UsuarioID = Cargamento.ClienteID");
            sql.Append("INNER JOIN Evento Envio on (Envio.CargamentoID = Cargamento.CargamentoID AND Envio.TipoEventoID = 1)");
            sql.Append("INNER JOIN Evento Recepcion on (Recepcion.CargamentoID = Cargamento.CargamentoID AND Recepcion.TipoEventoID = 2)");
            sql.Append("LEFT JOIN Evento Venta on (Venta.CargamentoID = Cargamento.CargamentoID AND Venta.TipoEventoID = 3)");
            sql.Append("LEFT JOIN Evento Decomisacion on (Decomisacion.CargamentoID = Cargamento.CargamentoID AND Decomisacion.TipoEventoID = 4)");
            sql.Append("INNER JOIN ItemMercaderia ItemRecepcion on (ItemRecepcion.MercaderiaID = Recepcion.MercaderiaID ) ");
            sql.Append("INNER JOIN Producto on (Producto.ProductoID = ItemRecepcion.ProductoID)");
            sql.Append("LEFT JOIN ItemMercaderia ItemEnvio on (ItemEnvio.MercaderiaID = Envio.MercaderiaID AND ItemEnvio.ProductoID = Producto.ProductoID)");
            sql.Append("LEFT JOIN ItemMercaderia ItemVenta on (ItemVenta.MercaderiaID = Venta.MercaderiaID AND ItemVenta.ProductoID = Producto.ProductoID)");  
            sql.Append("LEFT JOIN ItemMercaderia ItemDecomisacion on (ItemDecomisacion.MercaderiaID = Decomisacion.MercaderiaID AND ItemDecomisacion.ProductoID = Producto.ProductoID) ");
            sql.Append("WHERE Producto.ProductoID IS NOT NULL AND Cargamento.FechaEnvio BETWEEN @0 AND @1 ", FechaDesde, FechaHasta);

            if (!Cargamento.IsEmpty()) {
                sql.AppendKeywordMatching(this.Cargamento, "Cargamento.NumeroRemito","CONVERT(VARCHAR(10), Cargamento.FechaEnvio, 103)");
            }                
            if (!ProveedorID.IsEmpty())
                sql.Append("AND Cargamento.ProveedorID = @0", ProveedorID);
            else {
                if (!Proveedor.IsEmpty())
                    sql.AppendKeywordMatching(Proveedor, "usuario1.Nombre","usuario1.Username","usuario1.Email");
            }                
            if (!ClienteID.IsEmpty())
                sql.Append("AND Cargamento.ClienteID = @0", ClienteID);
            else {
                if (!Cliente.IsEmpty())
                    sql.AppendKeywordMatching(Cliente, "usuario2.Nombre","usuario2.Username","usuario2.Email");
            }
            if (!Producto.IsEmpty()) {
                sql.AppendKeywordMatching(this.Producto, "ISNULL(Convert(varchar(12),Producto.CodigoArticulo ),'') + ' - ' + ISNULL(Convert(varchar(50),Producto.Descripcion),'')");
            }
            sql.Append("GROUP BY ");
            sql.Append("Cargamento.NumeroRemito,");
            sql.Append("Cargamento.FechaEnvio,");
            sql.Append("Proveedor.Nombre,");
            sql.Append("Cliente.Nombre,");
            sql.Append("Producto.ProductoID,");
            sql.Append("Producto.CodigoArticulo,");
            sql.Append("Convert(varchar(50),Producto.Descripcion),");
            sql.Append("ItemEnvio.Cantidad,");
            sql.Append("ItemRecepcion.Cantidad,");
            sql.Append("(CASE WHEN Envio.Notas IS NULL	THEN '' ELSE ('Envío' + ' del ' + CONVERT(VARCHAR(10), Envio.Fecha, 103) + ' :\n'+ CAST(Envio.Notas as NVARCHAR(MAX)) +'\n\n') END)+(CASE WHEN Recepcion.Notas IS NULL  THEN '' ELSE('Recepcíon' + ' del ' + CONVERT(VARCHAR(10), Recepcion.Fecha, 103) + ' :\n' + CAST(Recepcion.Notas as NVARCHAR(MAX)) + '\n\n') END) + (CASE WHEN Venta.Notas IS NULL  THEN '' ELSE('Venta' + ' del ' + CONVERT(VARCHAR(10), Venta.Fecha, 103) + ' :\n' + CAST(Venta.Notas as NVARCHAR(MAX)) + '\n\n') END) +(CASE WHEN Decomisacion.Notas IS NULL THEN '' ELSE('Decomisacíon' + ' del ' + CONVERT(VARCHAR(10), Decomisacion.Fecha, 103) + ' :\n' + CAST(Decomisacion.Notas as NVARCHAR(MAX)) + '\n\n') END),");
            sql.Append("Cargamento.CargamentoID,");
            sql.Append("Producto.ProductoID");
            sql.Append("ORDER BY Cargamento.FechaEnvio,Cargamento.NumeroRemito  ASC");
            Resultado = DbHelper.CurrentDb().Fetch<ReporteActividadItem>(sql);
            Resultado.Reverse();
        }


        public class ReporteActividadItem {
            
            [Display(Name="Nro. Remito")]
            public int NumeroRemito { get; set; }

            [Display(Name="Fecha Envío")]
            public DateTime FechaEnvio { get; set; }

            [Display(Name="Proveedor")]
            public String Proveedor { get; set; }

            [Display(Name="Cliente")]
            public String Cliente { get; set; }
                        
            [Display(Name="Producto")]
            public String Articulo { get; set; }

            [Display(Name="Enviado")]
            public int Enviado { get; set; }

            [Display(Name="Recibido")]
            public int? Recibido { get; set; }

            [Display(Name="Vendido")]
            public int? Vendido { get; set; }

            [Display(Name="Decomisado")]
            public int? Decomisado { get; set; }            
            
            [Display(Name="Remanente")]
            public int? Remanente { get; set; }

            [Display(Name = "Precio Unitario Promedio ($/Un.)")]
            [DataType(DataType.Currency)]
            public Decimal PrecioUnitarioPromedio { get; set; }
            

            [Display(Name="Total ($)")]
		    [DataType(DataType.Currency)]
            public Decimal Total { get; set; }

            [Display(Name = "Observaciones")]
            [DataType(DataType.Currency)]
            public String Observaciones { get; set; }
                        
            public bool TieneArchivosAdjuntos { get; set; }

            public int CargamentoID { get; set; }

            public int ProductoID { get; set; }
            

        }

    }
}