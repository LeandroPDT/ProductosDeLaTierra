using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Text;
//using System.Web.WebPages.Html;

namespace Site.Models {
    public static class DecimalHelpers {
        public static bool IsEmpty(this Decimal Numero) {
            return Numero == 0;
        }
        public static bool IsEmpty(this Decimal? Numero) {
            return (Numero ?? 0) == 0;
        }
        public static string ToInvariantString(this Decimal Numero) {
            return Numero.ToString("0.####", CultureInfo.InvariantCulture);
        }
        public static string ToInvariantString(this Decimal Numero, int RoundingPlaces) {
            var places = new String('#', RoundingPlaces);
            return Numero.ToString("0." + places, CultureInfo.InvariantCulture);
        }
        public static string ToInvariantString(this double Numero) {
            return Numero.ToString("0.####", CultureInfo.InvariantCulture);
        }
        public static string ToInvariantString(this double Numero, int RoundingPlaces) {
            var places = new String('#', RoundingPlaces);
            return Numero.ToString("0." + places, CultureInfo.InvariantCulture);
        }
        public static decimal NullToZero(this Decimal? Numero) {
            return Numero == null ? 0 : (decimal)Numero;
        }
        public static string ToNiceCurrency(this Decimal? Numero) {
            return string.Format("{0:C2}", Numero == null ? 0 : (decimal)Numero);
        }
        public static string ToNiceCurrency(this Decimal Numero) {
            return string.Format("{0:C2}", Numero);
        }

        public static string To3DecimalesOptativo(this decimal Numero) {
            return Numero.ToString("###,###,###,###,##0.00#");
        }

        public static string To4DecimalesOptativo(this decimal Numero) {
            return Numero.ToString("###,###,###,###,##0.00##");
        }
		
        public static string SignoOCero(this Decimal? Numero) {
            return Numero == null ? "cero" : Numero < 0 ? "neg" : Numero == 0 ? "cero" : "pos";
        }
        public static string SignoOCero(this Decimal Numero) {
            return Numero < 0 ? "neg" : Numero == 0 ? "cero" : "pos";
        }
		
        public static string ToKiloFormat(this Decimal Numero) {
            return ((int)Numero).ToKiloFormat();
        }
        public static string ToKiloFormat(this Decimal? Numero) {
            return ((int)(Numero ?? 0)).ToKiloFormat();
        }
        public static string ToStringNoZero(this Decimal Numero, string Formato) {
            if (Numero == 0) return "";
            return Numero.ToString(Formato);
        }
        public static string ToStringNoZeroPorc(this Decimal Numero, string Formato) {
            if (Numero == 0) return "";
            return Numero.ToString(Formato) + "%";
        }
        public static string ToString(this Decimal? Numero, string Formato) {
            if (Numero == null) return "";
            return (Numero ?? 0).ToString(Formato);
        }
        public static decimal ToDecimal(this double Numero) {
            if (Numero > (double)decimal.MaxValue) return decimal.MaxValue;
            if (Numero < (double)decimal.MinValue) return decimal.MinValue;
            return (decimal)Numero;
        }
    }
    public static class IntHelpers {
        public static bool IsEmpty(this int Numero) {
            return Numero == 0;
        }
        public static bool IsEmpty(this int? Numero) {
            return (Numero ?? 0) == 0;
        }

        public static string ToSingleOrPlural(this int Numero, string Singular, string Plural) {
            if (Numero == 0) {
                return "No hay " + Plural;
            }
            else if (Numero == 1) {
                return "1 " + Singular;
            }
            else {
                return Numero.ToString() + " " + Plural;
            }
        }
        public static string ToKiloFormat(this int? num) {
            return (num ?? 0).ToKiloFormat();
        }
        public static string ToKiloFormat(this int num) {

            if (num >= 10000000)
                return (num / 1000000D).ToString("0.#") + "M";

            if (num >= 100000)
                return (num / 1000).ToString("N0") + "K";

            if (num >= 1000)
                return (num / 1000D).ToString("0.#") + "K";

            return num.ToString();
        }
        public static string ToStringNoZero(this int Numero) {
            if (Numero == 0) return "";
            return Numero.ToString();
        }

        public static string ToProgresivaFormat(this int? num) {
            return (num ?? 0).ToProgresivaFormat();
        }
        public static string ToProgresivaFormat(this int num) {
            var retval = (num).ToString("N0");
            return retval.Replace(".", "+");
        }

        // ya estan en bizlib
        //public static bool IsEmpty(this int Numero) {
        //    return (Numero == 0);
        //}
        //public static bool IsEmpty(this int? Numero) {
        //    return (Numero == null || Numero == 0);
        //}
    }


    public static class BooleanHelpers {
        public static HtmlString ToImage(this bool SiONo) {
            return new HtmlString(SiONo ? "<i class='icon-checkmark' style='color: darkgreen'></i>" : "<i class='icon-minus-2' style='color: silver'></i>");
        }
    }


    public static class DoubleHelpers {
        public static string ToString(this double? Numero, string Formato) {
            return ((decimal)(Numero ?? 0)).ToString(Formato);
        }
    }

    public static class FloatHelpers {
        public static string ToString(this float? Numero, string Formato) {
            return ((decimal)(Numero ?? 0)).ToString(Formato);
        }
        //    public static string ToInvariantString(this float Numero) {
    //        return Numero.ToString("#0.##", CultureInfo.InvariantCulture);
    //    }
        public static float NullToZero(this float? Numero) {
            return Numero == null ? 0 : (float)Numero;
        }
        public static bool IsEmpty(this float? Numero) {
            return (Numero == null || (float)Numero == 0);
        }
        public static bool IsEmpty(this float Numero) {
            return (Numero == 0);
        }
    }

    public static class StringHelpers
    {
        public static string ToCuit(this string Texto)
        {
            if (string.IsNullOrEmpty(Texto)) return "";
            if (Texto.Length < 11) return Texto;
            Texto = Texto.Replace("-", "").Replace(".", "");
            return Texto.Substring(0, 2) + "-" + Texto.Substring(2, 8) + "-" + Texto.Substring(10, 1);
        }
        public static string TextOverflow(this string Texto, int MaxChars, string EllipsisToShow = "...") {
            if (Texto != null && Texto.Length > MaxChars) {
                return Texto.Substring(0, MaxChars) + EllipsisToShow;
            }
            return Texto;
        }
        public static string Right(this string x, int NumberOfChars) {
            return x.Length >= NumberOfChars ? x.Substring(x.Length - NumberOfChars) : x;
        }
        public static string Left(this string x, int NumberOfChars) {
            return x.Length >= NumberOfChars ? x.Substring(0, NumberOfChars) : x;
        }

        public static decimal TryCDec(this string texto) {
            if (!string.IsNullOrEmpty(texto)) {
                string SepDecimal = null;
                string SepMiles = null;
                texto = texto.Trim().Replace("$", "");
                texto = texto.Trim().Replace(" ", "");
                if (Convert.ToDouble("3,24") == 324) {
                    SepDecimal = ".";
                    SepMiles = ",";
                }
                else {
                    SepDecimal = ",";
                    SepMiles = ".";
                }
                if (texto.IndexOf(SepDecimal) > 0) {
                    if (texto.IndexOf(SepMiles) > 0) {
                        if (texto.IndexOf(SepDecimal) > texto.IndexOf(SepMiles)) {
                            texto = texto.Replace(SepMiles, "");
                        }
                        else {
                            texto = texto.Replace( SepDecimal, "");
                            texto = texto.Replace(SepMiles, SepDecimal);
                        }
                    }
                }
                else {
                    texto = texto.Replace(SepMiles, SepDecimal);
                }
                decimal retval;
                if (Decimal.TryParse(texto, out retval)) {
                    return retval;
                }
            }
            return 0;
        }


        public static int? ZeroToNull(this int Numero) {
            return Numero == 0? null : (int?)Numero;
        }

        public static int? ZeroToNull(this int? Numero) {
            return Numero == 0 ? null : Numero;
        }


        public static Single TryCSingle(this string texto) {
            return Convert.ToSingle(texto.TryCDec());
        }

        public static string ToDashesIfEmpty(this string texto) {
            return string.IsNullOrEmpty(texto) ? "-----" : texto;
        }



        public static int TryCInt(this string texto) {
            if (string.IsNullOrEmpty(texto)) {
                return 0;
            }
            else {
                decimal Parcial = texto.TryCDec();
                return (int)Math.Round(Parcial, 0);
            }
        }

        public static bool IsValidPatente(this string Patente) {
            return (System.Text.RegularExpressions.Regex.IsMatch(Patente, "^[A-Z]{3}[0-9]{3}$"));
        }

        public static DateTime ISOToDate(this string s) {
            return new DateTime(Convert.ToInt32(s.Substring(0, 4)), Convert.ToInt32(s.Substring(4, 2)), Convert.ToInt32(s.Substring(7, 2)));
        }

        public static bool IsValidComprobante(this string Comprobante) {
            string[] separados = Comprobante.Split('-');
            if (separados.Length > 3) {
                return false;
            }
            else if (separados.Length == 3) {
                if (separados[0].Length > 1 || separados[1].Length > 4 || separados[2].Length > 8) {
                    return false;
                }
            }
            else if (separados.Length == 2) {
                if (separados[0].Length > 1 || separados[1].Length > 8) {
                    return false;
                }
            }
            else {
                if (separados[0].Length != 13) {
                    return false;
                }
            }
            return true;
        }

        public static string ToFormattedComprobante(this string Comprobante) {
            if (String.IsNullOrEmpty(Comprobante)) { return ""; };
            string[] separados = Comprobante.Split('-');
            if (separados.Length > 3) {
                return Comprobante;
            }
            else if (separados.Length == 3) {
                if (separados[0].Length > 1 || separados[1].Length > 4 || separados[2].Length > 8) {
                    return Comprobante;
                }
                else {
                    return separados[0].ToUpper() + "-" + separados[1].PadLeft(4, '0') + "-" + separados[2].PadLeft(8, '0');
                }
            }
            else if (separados.Length == 2) {
                if (separados[0].Length > 1 || separados[1].Length > 8) {
                    return Comprobante;
                }
                else {
                    return separados[0].ToUpper() + "-0000-" + separados[1].PadLeft(8, '0');
                }
            }
            else {
                if (separados[0].Length != 13) {
                    return Comprobante;
                }
                else {
                    return separados[0].Substring(0, 1).ToUpper() + "-" + separados[0].Substring(1, 4) + "-" + separados[0].Substring(5).PadLeft(8, '0');
                }
            }
        }

        public static string FormattedComprobante(string Comprobante) {
            return Comprobante.ToFormattedComprobante();
        }


        public static List<int> ToIntList(this string Texto) {
            var retval = new List<int>();
            if (!String.IsNullOrEmpty(Texto)) {
                var StringArray = Texto.Split(',');
                foreach (var item in StringArray) {
                    if (!String.IsNullOrEmpty(item) && item.TryCInt() > 0) {
                        retval.Add(item.TryCInt());
                    }
                }
            }
            return retval;
        }

        public static int[] ToIntArray(this string Texto) {
            return Texto.ToIntList().ToArray();
        }

        public static string ToNormalized(this string name) {
            string normalizedString = name.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();


            foreach (char c in normalizedString) {
                switch (CharUnicodeInfo.GetUnicodeCategory(c)) {

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        stringBuilder.Append(c);
                        break;
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DashPunctuation:
                        stringBuilder.Append("-");
                        break;
                }
            }

            string retval = stringBuilder.ToString();
            char[] separator = { '-' };
            return string.Join("-", retval.Split(separator, StringSplitOptions.RemoveEmptyEntries));
        }


    }

    public static class EnumHelpers {
        public static String convertToString(this Enum eff) {
            return Enum.GetName(eff.GetType(), eff);
        }

        public static EnumType converToEnum<EnumType>(this String enumValue) {
            return (EnumType)Enum.Parse(typeof(EnumType), enumValue);
        }

    }
    public static class DateHelpers {
        public static DateTime FirstDayOfMonth(this DateTime Fecha) {
            return new DateTime(Fecha.Year, Fecha.Month, 1);
        }
        public static DateTime LastDayOfMonth(this DateTime Fecha) {
            return Fecha.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        public static string ToISO(this DateTime Fecha) {
            return Fecha.ToString("yyyyMMdd");
        }
        public static string ToISO(this DateTime? Fecha) {
            if (Fecha == null) return "";
            return ((DateTime)Fecha).ToISO();
        }
        public static string ToISO8601(this DateTime Fecha) {
            return Fecha.ToString("yyyy-MM-ddTHH:mm:ss-03:00");
        }
        public static string ToISO8601(this DateTime? Fecha) {
            if (Fecha == null) return "";
            return ((DateTime)Fecha).ToISO8601();
        }
        public static string ToShortDateTimeString(this DateTime Fecha) {
            return Fecha.ToShortDateString() + " " + Fecha.ToShortTimeString();
        }
        public static string ToShortDateTimeString(this DateTime? Fecha) {
            if (Fecha == null) return "";
            return Fecha.ToShortDateString() + " " + Fecha.ToShortTimeString();
        }


        public static string ToMonthString(this DateTime Mes) {
            return Mes.ToString("MM/yyyy");
        }
        public static string ToMonthString(this DateTime? Mes) {
            if (Mes == null) {
                return "";
            }
            else {
                return (Mes ?? DateTime.Today).ToString("MM/yyyy");
            }
        }
        public static string ToShortDateString(this DateTime? fecha) {
            if (fecha == null) {
                return "";
            }
            else {
                return (fecha ?? DateTime.Today).ToShortDateString();
            }
        }

        public static string ToShortTimeString(this DateTime? fecha) {
            if (fecha == null) {
                return "";
            }
            else {
                return (fecha ?? DateTime.Today).ToString("HH:mm");
            }
        }

        public static string ToDDMMM(this DateTime fecha) {
            return fecha.ToString("dd MMM"); 
        }

        public static string ToDDMMM(this DateTime? fecha) {
            if (fecha == null) {
                return "";
            }
            else {
                return (fecha ?? DateTime.Today).ToDDMMM();
            }
        }


        public static string ToHHMM(this DateTime? fecha) {
            if (fecha == null) {
                return "0:00";
            }
            else {
                return (fecha ?? DateTime.Today).ToString("HH:mm");
            }
        }

        public static string ToHHMM(this DateTime fecha) {
            return fecha.ToString("HH:mm");
        }

        public static bool IsEmpty(this DateTime? fecha) {
            return (fecha == null || (DateTime)fecha == DateTime.MinValue);
        }

        public static bool IsEmpty(this DateTime fecha) {
            return ((DateTime)fecha == DateTime.MinValue);
        }

        public static List<DateTime> ListHastaMes(this DateTime? MesDesde, DateTime? MesHasta) {
            var retval = new List<DateTime>();
            if (MesDesde == null) { return new List<DateTime>(); };
            for (DateTime Mes = (DateTime)MesDesde; Mes <= MesHasta; Mes = Mes.AddMonths(1)) {
                retval.Add( Mes );
            }
            return retval;
        }

        public static List<DateTime> ListHastaMes(this DateTime MesDesde, DateTime MesHasta) {
            var retval = new List<DateTime>();
            for (DateTime Mes = (DateTime)MesDesde; Mes <= MesHasta; Mes = Mes.AddMonths(1)) {
                retval.Add(Mes);
            }
            return retval;
        }
        public static string ToTrimestreString(this DateTime Mes) {
            int Trimestre = ((Mes.Month - 1) / 3) + 1;
            return "T" + Trimestre.ToString() + " " + Mes.Year.ToString();
        }
        public static string ToTrimestreCode(this DateTime Mes) {
            int Trimestre = ((Mes.Month - 1) / 3) + 1;
            return Trimestre.ToString() + Mes.Year.ToString();
        }
        public static string ToJS(this DateTime Fecha) {
            return string.Format("Date.UTC({0},{1},{2})", Fecha.Year, Fecha.Month - 1, Fecha.Day);
        }
        public static string ToMMDDYYYY(this DateTime Fecha) {
            return Fecha.ToString("MM/dd/yyyy");
        }


        public static DateTime SetHora(this DateTime Fecha, DateTime Hora) {
            return Fecha.Date.AddHours(Hora.Hour).AddMinutes(Hora.Minute).AddSeconds(Hora.Second);
        }
        public static DateTime SetHora(this DateTime Fecha, DateTime? Hora) {
            if (Hora == null) {
                return Fecha.Date;
            }
            return Fecha.SetHora((DateTime)Hora);
        }
        public static DateTime ToDatelessDateTime(this DateTime? Fecha) {
            if (Fecha == null) {
                return new DateTime(1899, 12, 30, 0, 0, 0);
            }
            return (Fecha ?? DateTime.Today).ToDatelessDateTime();
        }

        public static DateTime ToDatelessDateTime(this DateTime Fecha) {
            // el vb3 guardaba el time con la fecha 1899-12-30. Es da problemas en el viejo al comparar horas.
            return new DateTime(1899, 12, 30, Fecha.Hour, Fecha.Minute, 0);
        }

        public static string TimeAgo(this DateTime Fecha) {
            dynamic ts = new TimeSpan(DateTime.Now.Ticks - Fecha.Ticks);
            double delta = ts.TotalSeconds;

            if (delta < 60) {
                return ts.Seconds == 1 ? "recién" : "Hace unos segundos";
            }
            if (delta < 120) {
                return "hace un minuto";
            }
            if (delta < 2700) {
                // 45 * 60
                return "hace " + ts.Minutes + " minutos";
            }
            if (delta < 5400) {
                // 90 * 60
                return "hace una hora";
            }
            if (delta < 86400) {
                // 24 * 60 * 60
                return "hace " + ts.Hours + " horas";
            }
            if (delta < 172800) {
                // 48 * 60 * 60
                return "ayer";
            }
            return "el " + Fecha.Day.ToString() + " de " + Fecha.ToString("MMM") + (Fecha.Year == System.DateTime.Now.Year ? "" : " de " + Fecha.Year.ToString()) + " a las " + Fecha.ToShortTimeString();

        }



        


        

    }
    public static class ControllerHelpers {
        public static string RenderViewToString(this ControllerBase controller, string viewName, object model) {
            ControllerContext context = controller.ControllerContext;
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            ViewDataDictionary viewData = new ViewDataDictionary(model);

            using (System.IO.StringWriter sw = new System.IO.StringWriter()) {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                ViewContext viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
    public static class GUIDHelpers {
        public static bool IsEmpty(this Guid ElGuid) {
            return (ElGuid == Guid.Empty);
        }
        public static bool IsEmpty(this Guid? ElGuid) {
            return (ElGuid == null || ElGuid == Guid.Empty);
        }
    }

    public static class ArrayHelpers {
        public static string ToJSArray(this String[] Cosas) {
            var builder = new StringBuilder();
            var first = true;
            foreach (string value in Cosas) {
                if (!first) builder.Append(",");
                builder.Append("'" + value + "'");
                first = false;
            }
            return "[" + builder.ToString() + "]";
        }
        public static string ToJSArray(this int?[] Cosas) {
            var builder = new StringBuilder();
            var first = true;
            foreach (int? value in Cosas) {
                if (!first) builder.Append(",");
                if (value == null) {
                    builder.Append("null");
                }
                else {
                    builder.Append(value.ToString());
                }
                first = false;
            }
            return "[" + builder.ToString() + "]";
        }
        public static string ToJSArray(this int[] Cosas) {
            var builder = new StringBuilder();
            var first = true;
            foreach (int value in Cosas) {
                if (!first) builder.Append(",");
                builder.Append(value.ToString());
                first = false;
            }
            return "[" + builder.ToString() + "]";
        }

        public static string ToJSArray(this decimal?[] Cosas) {
            return Cosas.ToJSArray(4);
        }
        public static string ToJSArray(this decimal?[] Cosas, int RoundingPlaces) {
            var builder = new StringBuilder();
            var first = true;
            foreach (decimal? value in Cosas) {
                if (!first) builder.Append(",");
                if (value == null) {
                    builder.Append("null");
                }
                else {
                    builder.Append((value ?? 0).ToInvariantString(RoundingPlaces));
                }
                first = false;
            }
            return "[" + builder.ToString() + "]";
        }
        
        public static string ToJSArray(this decimal[] Cosas) {
            return Cosas.ToJSArray(4);
        }
        public static string ToJSArray(this decimal[] Cosas, int RoundingPlaces) {
            var builder = new StringBuilder();
            var first = true;
            foreach (decimal value in Cosas) {
                if (!first) builder.Append(",");
                builder.Append(value.ToInvariantString(RoundingPlaces));
                first = false;
            }
            return "[" + builder.ToString() + "]";
        }
        public static string ToJSArray(this DateTime?[] Cosas) {
            var builder = new StringBuilder();
            var first = true;
            foreach (DateTime? value in Cosas) {
                if (!first) builder.Append(",");
                if (value == null) {
                    builder.Append("null");
                }
                else {
                    var Fecha = value ?? DateTime.MinValue;
                    builder.AppendFormat("Date.UTC({0}, {1}, {2})", Fecha.Year, Fecha.Month - 1, Fecha.Day);
                }
                first = false;
            }
            return "[" + builder.ToString() + "]";
        }
        public static string ToJSArray(this DateTime[] Cosas) {
            var builder = new StringBuilder();
            var first = true;
            foreach (DateTime value in Cosas) {
                if (!first) builder.Append(",");
                var Fecha = value;
                builder.AppendFormat("Date.UTC({0},{1},{2})", Fecha.Year, Fecha.Month-1, Fecha.Day);
                first = false;
            }
            return "[" + builder.ToString() + "]";
        }
        public static int[] ToIntArray(this String[] Cosas) {
            int[] retval = new int[Cosas.Length];
            for (int i = 0; i < Cosas.Length; i++) {
                retval[i] = Cosas[i].TryCInt();
			 
			}
            return retval;
        }
        public static string ToCommaSeparatedString(this int[] array) {
            return String.Join(",", new List<int>(array).ConvertAll(i => i.ToString()).ToArray());
        }
    }
    public static class IDNombreParHelpers {
        public static string ToJSON(this List<IDNombrePar> Lista) {
            var builder = new StringBuilder();
            var first = true;
            foreach (IDNombrePar value in Lista) {
                if (!first) builder.Append(",");
                var Fecha = value;
                builder.Append("{ID: " + value.ID.ToString() + ", Nombre: '" + value.Nombre + "'}");
                first = false;
            }
            return "[" + builder.ToString() + "]";
        }
    }

    public static class EnumerableHelpers {
        public static T MinOrDefault<T>(this IEnumerable<T> sequence) {
            if (sequence.Any()) {
                return sequence.Min();
            }
            else {
                return default(T);
            }
        }
        public static T MinOrZero<T>(this IEnumerable<T> sequence) {
            if (sequence.Any()) {
                return sequence.Min();
            }
            else {
                return (T)Convert.ChangeType(0, typeof(T));
            }
        }
        public static T MaxOrZero<T>(this IEnumerable<T> sequence) {
            if (sequence.Any()) {
                return sequence.Max();
            }
            else {
                return (T)Convert.ChangeType(0, typeof(T));
            }
        }
        // No funciona. Directamete aplicar ?? 0 al resultado
        //public static T SumOrZero<T>(this IEnumerable<T> sequence) {
        //    if (sequence.Any()) {
        //        return sequence.Sum();
        //    }
        //    else {
        //        return (T)Convert.ChangeType(0, typeof(T));
        //    }
        //}
        public static decimal AverageOrZero(this IEnumerable<decimal> sequence) {
            if (sequence.Any()) {
                return sequence.Average();
            }
            return 0;
        }
    }

    public static class TimeSpanHelpers {
        public static string ToHHMM(this TimeSpan? hora) {
            if (hora == null) {
                return "0:00";
            }
            else {
                return (hora ?? new TimeSpan()).ToHHMM();
            }
        }

        public static string ToHHMM(this TimeSpan hora) {
            return hora.ToString(@"hh\:mm");
        }
        

    }

    public static class SelectListHelpers {
        public static List<SelectListItem> With(this List<SelectListItem> ElSelectList, SelectListItem ElItem) {
            ElSelectList.Add(ElItem);
            return ElSelectList;
        }

        public static void Shuffle<T>(this IList<T> list) {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }

    public static class EmailHelpers {
        public static void Send(this System.Net.Mail.MailMessage email) {
            var smtp = new System.Net.Mail.SmtpClient();
            smtp.Send(email);
        }
    }


    public static class ModelStateHelpers {
        public static string ToHTMLString(this ModelStateDictionary MS) {
            string retval = "";
            foreach (ModelState item in MS.Values) {
                foreach (ModelError error in item.Errors) {
                    retval += error.ErrorMessage + "; " + Environment.NewLine;
                }
            }
            return retval;
        }
        public static void AddModelWarning(this ModelStateDictionary ModelState, string Mensaje) {
            var WarningList = (List<string>)HttpContext.Current.Items["ModelStateWarnings"];
            if (WarningList == null) {
                WarningList = new List<string>();
                HttpContext.Current.Items["ModelStateWarnings"] = WarningList;
            }
            WarningList.Add(Mensaje);
        }
        public static List<string> GetWarnings(this ModelStateDictionary ModelState) {
            var WarningList = (List<string>)HttpContext.Current.Items["ModelStateWarnings"];
            if (WarningList == null) {
                WarningList = new List<string>();
            }
            return WarningList;
        }

    }



}