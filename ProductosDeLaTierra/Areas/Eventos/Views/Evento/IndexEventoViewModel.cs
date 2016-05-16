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
    public class IndexEventoViewModel {
                
        public string q { get; set; }
        public int CantidadPorPagina { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int? ProveedorID { get; set; }
        public String Proveedor{ get; set; }
        public int? ClienteID { get; set; }
        public String Cliente{ get; set; }
        public String Tipo{ get; set; }
        
        public List<ListarEventoViewModel> Resultado { get; set;}

        public IndexEventoViewModel() {
            q = Sitio.GetPref("Evento-q", "");
            CantidadPorPagina = Sitio.GetPref("Evento-CantidadPorPagina", 50);
            FechaDesde = Sitio.GetPref("Evento-FechaDesde", new DateTime(DateTime.Today.Year, 1, 1));//DateTime.Today.FirstDayOfMonth()
            FechaHasta = Sitio.GetPref("Evento-FechaHasta", DateTime.Today);
            ProveedorID = Sitio.EsEmpleado || Usuario.HasRol(Sitio.Usuario.UsuarioID,"Cliente")? Sitio.GetPref("Evento-ProveedorID", "").AsInt().ZeroToNull():Sitio.Usuario.ProveedorID.IsEmpty()?Sitio.Usuario.UsuarioID:Sitio.Usuario.ProveedorID;
            Proveedor = Sitio.GetPref("Evento-Proveedor", "");
            ClienteID =  Sitio.EsEmpleado || Usuario.HasRol(Sitio.Usuario.UsuarioID,"Proveedor")? Sitio.GetPref("Evento-ClienteID", "").AsInt().ZeroToNull():Sitio.Usuario.ProveedorID.IsEmpty()?Sitio.Usuario.UsuarioID:0;
            Cliente = Sitio.GetPref("Evento-Cliente", "");
            Tipo = Sitio.GetPref("Evento-Tipo", "");
        }

        public void SetPref() {
            Sitio.SetPref("Evento-CantidadPorPagina", CantidadPorPagina);
            Sitio.SetPref("Evento-q", q);
            Sitio.SetPref("Evento-FechaDesde", FechaDesde);
            Sitio.SetPref("Evento-FechaHasta", FechaHasta);
            Sitio.SetPref("Evento-ProveedorID", ProveedorID??0);
            Sitio.SetPref("Evento-Proveedor", Proveedor);
            Sitio.SetPref("Evento-ClienteID", ClienteID??0);
            Sitio.SetPref("Evento-Cliente", Cliente);
        }

        public void CalcResultado() {
            ProveedorID = ProveedorID.ZeroToNull();
            ClienteID = ClienteID.ZeroToNull();
            var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(this.CantidadPorPagina);
            sql.Append("Evento.*, Cargamento.NumeroRemito as NumeroRemito, Cargamento.FechaEnvio as FechaEnvio, usuario1.Nombre as Proveedor, usuario2.Nombre as Cliente");
            sql.Append("FROM Evento");
            sql.Append("INNER JOIN Cargamento ON Cargamento.CargamentoID = Evento.CargamentoID");
            sql.Append("INNER JOIN Usuario usuario1 ON Cargamento.ProveedorID = usuario1.UsuarioID");
            sql.Append("INNER JOIN Usuario usuario2 ON Cargamento.ClienteID = usuario2.UsuarioID");
            sql.Append("WHERE Evento.Fecha BETWEEN @0 AND @1 ", FechaDesde, FechaHasta);
            if (!q.IsEmpty()) {
                sql.AppendKeywordMatching(this.q, "Cargamento.NumeroRemito","CONVERT(VARCHAR(10), Cargamento.FechaEnvio, 103)");
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

            if (!Tipo.IsEmpty()) {
                sql.Append("AND Evento.TipoEventoID = @0", new EventoTipo(Tipo).ID);
            }
            else {
                // solo busco los que el usuario puede acceder
                String queryAux = "(";
                var unaccessibleTypesIDs = new List<int>();
                foreach (EventoTipo type in EventoTipo.Lista()) {
                    if (!Seguridad.CanAccess(type.Permiso)) { 
                        unaccessibleTypesIDs.Add(type.ID);
                        queryAux+=(type.ID.ToString()+",");
                    }
                }
                queryAux = queryAux.Substring(0, queryAux.Length - 1)+")";
                if (unaccessibleTypesIDs.Count>0)
                    sql.Append("AND Evento.TipoEventoID NOT IN "+queryAux);
            }
            sql.Append("ORDER BY Evento.Fecha DESC, Evento.CargamentoID DESC");
            Resultado = DbHelper.CurrentDb().Fetch<ListarEventoViewModel>(sql);
        }
        

        public class ListarEventoViewModel{
            
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
		    public int CargamentoID{ get; set; }         

         
            public int NumeroRemito{get;set;}
            public DateTime FechaEnvio{get;set;}
            public String Proveedor{get;set;}
            public String Cliente{get;set;}
            
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
            
            [ResultColumn]
            public String Cargamento {
                get {
                    return "Cargamento Nro. "+ NumeroRemito.ToString();// + FechaEnvio.ToShortDateString()+ " - Nro. remito " 
                }
            }

        }

    }
}