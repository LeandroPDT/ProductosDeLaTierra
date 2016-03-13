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
    public class IndexCargamentoViewModel {
                
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public String TipoVenta{ get; set; }
        public String Estado{ get; set; }        
        public int? ProveedorID { get; set; }
        public String Proveedor{ get; set; }
        public int? ClienteID { get; set; }
        public String Cliente{ get; set; }
        public List<Cargamento> Resultado { get; set;}

        public IndexCargamentoViewModel() {
            q = Sitio.GetPref("Cargamento-q", "");
            CantidadPorPagina = Sitio.GetPref("Cargamento-CantidadPorPagina", 50);
            FechaDesde = Sitio.GetPref("Cargamento-FechaDesde", DateTime.Today.FirstDayOfMonth());
            FechaHasta = Sitio.GetPref("Cargamento-FechaHasta", DateTime.Today);
            ProveedorID = Sitio.EsEmpleado || Usuario.HasRol(Sitio.Usuario.UsuarioID,"Cliente")? Sitio.GetPref("Cargamento-ProveedorID", "").AsInt().ZeroToNull():Sitio.Usuario.UsuarioID;
            Proveedor = Sitio.GetPref("Cargamento-Proveedor", "");
            ClienteID =  Sitio.EsEmpleado || Usuario.HasRol(Sitio.Usuario.UsuarioID,"Proveedor")? Sitio.GetPref("Cargamento-ClienteID", "").AsInt().ZeroToNull():Sitio.Usuario.UsuarioID;
            Cliente = Sitio.GetPref("Cargamento-Cliente", "");
            TipoVenta = Sitio.GetPref("Cargamento-TipoVenta", "");
            Estado= Sitio.GetPref("Cargamento-Estado", "");
        }

        public void SetPref() {
            Sitio.SetPref("Cargamento-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("Cargamento-q", q);
            Sitio.SetPref("Cargamento-FechaDesde", FechaDesde);
            Sitio.SetPref("Cargamento-FechaHasta", FechaHasta);
            Sitio.SetPref("Cargamento-ProveedorID", ProveedorID??0);
            Sitio.SetPref("Cargamento-Proveedor", Proveedor);
            Sitio.SetPref("Cargamento-ClienteID", ClienteID??0);
            Sitio.SetPref("Cargamento-Cliente", Cliente);
            Sitio.SetPref("Cargamento-TipoVenta", TipoVenta);
            Sitio.SetPref("Cargamento-Estado", Estado);
        }

        public void CalcResultado() {
            ProveedorID = ProveedorID.ZeroToNull();
            ClienteID = ClienteID.ZeroToNull();
            var sql = Cargamento.BaseQuery(this.CantidadPorPagina);
            sql.Append("WHERE Cargamento.FechaEnvio BETWEEN @0 AND @1 ", FechaDesde, FechaHasta);
            if (!q.IsEmpty()) {
                sql.AppendKeywordMatching(this.q, "NumeroRemito","FechaEnvio");
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

            Cargamento.AppendEstadoMatching(sql, Estado);
            Cargamento.AppendTipoVentaMatching(sql, TipoVenta);

            Resultado = DbHelper.CurrentDb().Fetch<Cargamento>(sql);
            Resultado.Reverse();
        }
        
    }
}