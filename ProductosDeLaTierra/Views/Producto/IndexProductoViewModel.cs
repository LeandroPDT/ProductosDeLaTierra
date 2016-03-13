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
    public class IndexProductoViewModel {
                
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public int? UsuarioID { get; set; }
        public String Usuario{ get; set; }
        public int? ProductoID { get; set; }

        public List<Producto> Resultado { get; set;}

        public IndexProductoViewModel() {
            q = Sitio.GetPref("Producto-q", "");
            CantidadPorPagina = Sitio.GetPref("Producto-CantidadPorPagina", 50);
            UsuarioID = Sitio.EsEmpleado? Sitio.GetPref("Producto-UsuarioID", "").AsInt().ZeroToNull():Sitio.Usuario.UsuarioID;
            Usuario = Sitio.GetPref("Producto-Usuario", "");
        }

        public void SetPref() {
            Sitio.SetPref("Producto-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("Producto-q", q);
            Sitio.SetPref("Producto-UsuarioID", UsuarioID??0);
            Sitio.SetPref("Producto-Usuario", Usuario);
        }

        public void CalcResultado() {
            UsuarioID = UsuarioID.ZeroToNull();
            var sql = Producto.BaseQuery(this.CantidadPorPagina);
            sql.Append("WHERE 1=1");
            if (!q.IsEmpty()) {
                sql.AppendKeywordMatching(this.q, "ISNULL(Convert(varchar(12),Producto.CodigoArticulo ),'') + ' - ' + ISNULL(Convert(varchar(50),Descripcion),'')");
            }
                
            if (!UsuarioID.IsEmpty())
                sql.Append("AND Producto.UsuarioID = @0", UsuarioID);
            else {
                if (!Usuario.IsEmpty())
                    sql.AppendKeywordMatching(Usuario, "usuario.Nombre","usuario.Username","usuario.Email");
            }

            Resultado = DbHelper.CurrentDb().Fetch<Producto>(sql);
            Resultado.Reverse();
        }
    }
}