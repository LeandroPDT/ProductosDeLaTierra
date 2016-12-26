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
    public class IndexCobranzaViewModel
    {
                
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int? UsuarioID { get; set; }
        public String Usuario{ get; set; }
        public int? CobranzaID { get; set; }

        public List<Cobranza> Resultado { get; set;}

        public IndexCobranzaViewModel() {
            q = Sitio.GetPref("Cobranza-q", "");
            CantidadPorPagina = Sitio.GetPref("Cobranza-CantidadPorPagina", 50);
            FechaDesde = new DateTime(DateTime.Today.Year, 1, 1);
            FechaHasta = DateTime.Today;
            Sitio.SetPref("Cobranza-FechaDesde", FechaDesde);
            Sitio.SetPref("Cobranza-FechaHasta", FechaHasta);
            UsuarioID = Sitio.EsEmpleado? Sitio.GetPref("Cobranza-UsuarioID", "").AsInt().ZeroToNull():Sitio.Usuario.ProveedorID.IsEmpty()?Sitio.Usuario.UsuarioID:Sitio.Usuario.ProveedorID;
            Usuario = Sitio.GetPref("Cobranza-Usuario", "");
        }

        public void SetPref() {
            Sitio.SetPref("Cobranza-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("Cobranza-q", q);
            Sitio.SetPref("Cobranza-UsuarioID", UsuarioID??0);
            Sitio.SetPref("Cobranza-Usuario", Usuario);
        }

        public void CalcResultado() {
            UsuarioID = UsuarioID.ZeroToNull();
            var sql = Cobranza.BaseQuery(this.CantidadPorPagina);
            sql.Append("WHERE Cobranza.Fecha BETWEEN @0 AND @1 ", FechaDesde, FechaHasta);
            if (!q.IsEmpty()) {
                sql.AppendKeywordMatching(this.q, "Cobranza.Referencia");
            }
                
            if (!UsuarioID.IsEmpty())
                sql.Append("AND Cobranza.ProveedorID = @0", UsuarioID);
            else {
                if (!Usuario.IsEmpty())
                    sql.AppendKeywordMatching(Usuario, "usuario.Nombre","usuario.Username","usuario.Email");
            }

            Resultado = DbHelper.CurrentDb().Fetch<Cobranza>(sql);
            Resultado.Reverse();
        }
    }
}