using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

namespace Site.Models {
    public class Intervalo {

        public const int Mes = 1;
        public const int Trimestre = 2;
        public const int Anio = 3;

        public int Codigo { set; get; }
        public string Nombre { set; get; }
        public DateTime FechaDesde { set; get; }
        public DateTime FechaHasta { set; get; }

        public Intervalo() {
        }

        public Intervalo(int Codigo, DateTime FechaDesde, DateTime FechaHasta) {
            this.Codigo = Codigo;
            this.FechaDesde = FechaDesde;
            this.FechaHasta = FechaHasta;
        }

        public string tsqlSelect(string NombreCampo) {
            if (Codigo == Intervalo.Mes) {
                return string.Format("Convert(varchar(6), {0}, 112)", NombreCampo);
            }
            else if (Codigo == Intervalo.Trimestre) {
                return string.Format("Convert(varchar(1), datepart(qq, {0}))+Convert(varchar(4), datepart(yyyy, {0}))", NombreCampo);
            }
            else {
                return string.Format("Convert(varchar(4), datepart(yyyy, {0}))", NombreCampo);
            }
        }

        public IEnumerable<string> ListaPeriodos(bool PrettyOutput = false) {
            var Retval = new List<string>();
            if (Codigo == Intervalo.Mes) {
                for (DateTime Mes = FechaDesde; Mes <= FechaHasta; Mes = Mes.AddMonths(1)) {
                    yield return (PrettyOutput ? Mes.ToMonthString() : Mes.ToString("yyyyMM"));
                }
            }
            else if (Codigo == Intervalo.Trimestre) {
                for (DateTime Mes = FechaDesde; Mes <= FechaHasta; Mes = Mes.AddMonths(3)) {
                    yield return (PrettyOutput ? Mes.ToTrimestreString() : Mes.ToTrimestreCode());
                }
            }
            else {
                for (int a = FechaDesde.Year; a <= FechaHasta.Year; a++) {
                    yield return a.ToString();
                }
            }
        }

        public string PrettyOutput(string Periodo) {
            if (Codigo == Intervalo.Mes) {
                return Periodo.Substring(4, 2) + "/" + Periodo.Substring(0, 4);
            }
            else if (Codigo == Intervalo.Trimestre) {
                return "T" + Periodo.Substring(0, 1) + " " + Periodo.Substring(1, 4);
            }
            else {
                return Periodo;
            }
        }


        public static List<Intervalo> GetList(string IncludeAllItemstext) {
            List<Intervalo> retval = new List<Intervalo>();
            if (!IncludeAllItemstext.IsEmpty()) {
                retval.Add(new Intervalo { Nombre = IncludeAllItemstext, Codigo = 0 });
            }
            retval.Add(new Intervalo { Codigo = Mes, Nombre = "Mes" });
            retval.Add(new Intervalo { Codigo = Trimestre, Nombre = "Trimestre" });
            retval.Add(new Intervalo { Codigo = Anio, Nombre = "Año" });
            return retval;
        }



        public static SelectList GetSelectList(int ValorActualSeleccionado, string IncludeAllItemstext = "") {
            return new SelectList(Intervalo.GetList(IncludeAllItemstext), "Codigo", "Nombre", ValorActualSeleccionado);
        }



    }
}

