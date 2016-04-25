using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;
using OfficeOpenXml;
using System.Web.Helpers;
using System.Web.WebPages;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Reflection;
using System.Dynamic;
using Microsoft.Internal.Web.Utils;

namespace Site.Models {
    public class ExcelResult: ActionResult {

        private const BindingFlags BindFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase;

        private string _fileName;
        private string _title;
        private System.Web.Helpers.BizGridColumn[] _cols;
        private IEnumerable<dynamic> _source = null;
        private ExcelPackage _pck;


        public ExcelResult(string title, IEnumerable<dynamic> source, System.Web.Helpers.BizGridColumn[] cols) {
            _fileName = GetFileNameFromString(title) + "_" + Utils.GetRandomPasswordUsingGUID(8) + ".xlsx";
            _title = title;
            _source = source;
            _cols = cols;
        }

        public ExcelResult(string title, ExcelPackage pck) {
            _fileName = GetFileNameFromString(title) + "_" + Utils.GetRandomPasswordUsingGUID(8) + ".xlsx";
            _title = title;
            _pck = pck;
        }

        public override void ExecuteResult(ControllerContext context) {
            if (_pck == null) {
                _pck = new ExcelPackage();
                
                //Create the worksheet
                ExcelWorksheet ws = _pck.Workbook.Worksheets.Add(_title);

                int c = 1;
                foreach (BizGridColumn column in _cols) {
                    var cell = ws.Cells[1, c];
                    //ws.SetValue(1, c, column.Header ?? column.ColumnName);
                    cell.Value = column.Header ?? column.ColumnName;
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    cell.Style.Font.Color.SetColor(Color.White);
                    c++;
                }


                int r = 2; // arranco desde la segunda porque la primera tiene el titulo
                foreach (object row in _source) {
                    c = 1;
                    foreach (var column in _cols) {
                        object value;
                        var _dynamic = row as IDynamicMetaObjectProvider;
                        TryGetMember(row, _dynamic, column.ColumnName.ToString(), out value);
                        //ws.SetValue(r, c, StandardFormat(column, value));
                        ws.SetValue(r, c, CastIfNeeded(value));
                        //ws.SetValue(r, c, value ?? string.Empty);
                        //ws.Cells["A1"].Style.Numberformat.Format = "#,##0.00";
                        //ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        c++;
                    }
                    r++;
                }

                // hago autofit en las columnas
                c = 1;
                foreach (BizGridColumn column in _cols) {
                    ws.Column(c).AutoFit();
                    c++;
                }
            }

            //Write it back to the client
            // write the file to the response stream
            HttpContext con = HttpContext.Current;
            con.Response.Clear();
            con.Response.AddHeader("content-disposition", "attachment;filename=" + _fileName);
            con.Response.Charset = "";
            con.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            con.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            con.Response.BinaryWrite(_pck.GetAsByteArray());
            con.Response.End();
        }
        private static object CastIfNeeded(object Valor) {
            if (Valor == null) return "";
            if (Valor.GetType() == typeof(DateTime)) {
                return ((DateTime)Valor).ToShortDateString();
            }
            return Valor;
        }


        private static string StandardFormat(BizGridColumn column, object Valor) {
            if (Valor == null) return "";
            if (Valor.GetType() == typeof(decimal) || Valor.GetType() == typeof(float)) {
                column.StandardStyle = "numericcell";
                return String.Format("{0:N2}", Valor);

            }
            else if (Valor.GetType() == typeof(bool)) {
                return ((bool)Valor ? "Si" : "No");
            }
            else if (Valor.GetType() == typeof(DateTime)) {
                if (column.Style == "Vencimiento") {
                    if (((DateTime)Valor) <= DateTime.Today) {
                        column.StandardStyle = "vencidocell";
                    }
                    else if (((DateTime)Valor) <= DateTime.Today.AddDays(7)) {
                        column.StandardStyle = "porvencercell";
                    }
                    else {
                        column.StandardStyle = "";
                    }
                }
                return ((DateTime)Valor).ToShortDateString();
            }
            else {
                return Valor.ToString();
            }
        }

        private static HelperResult Format(Func<dynamic, object> format, dynamic arg) {
            var result = format(arg);
            return new HelperResult(tw => {
                var helper = result as HelperResult;
                if (helper != null) {
                    helper.WriteTo(tw);
                    return;
                }
                IHtmlString htmlString = result as IHtmlString;
                if (htmlString != null) {
                    tw.Write(htmlString);
                    return;
                }
                if (result != null) {
                    tw.Write(HttpUtility.HtmlEncode(result));
                }
            });
        }

        private string GetFileNameFromString(string Texto ) {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) {
                Texto = Texto.Replace(c, '_');
            }
            return Texto;
        }


        private bool TryGetMember(object obj, IDynamicMetaObjectProvider dynamicMetaData, string name, out object result) {
            try {
                // Try to evaluate the dynamic member based on the name
                if (dynamicMetaData != null && DynamicHelper.TryGetMemberValue(dynamicMetaData, name, out result)) {
                    return true;
                } 

                PropertyInfo property = obj.GetType().GetProperty(name, BindFlags);
                if ((property != null) && (property.GetIndexParameters().Length == 0)) {
                    result = property.GetValue(obj, null);
                    return true;
                }
                result = null;
                return false;
            }
            catch (Exception) {
                throw new InvalidOperationException("No se encontró la columna " + name); 
            } 
        }


    }
}
