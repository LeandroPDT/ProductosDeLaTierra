using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Collections;

namespace Site.Models {
    public class HelperPage : System.Web.WebPages.HelperPage {
        // Workaround - exposes the MVC HtmlHelper instead of the normal helper
        public static new HtmlHelper Html {
            get { return ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).Html; }
        }

    }

    public static class RazorExternalHelpersExtensions {
        public static HtmlHelper GetPageHelper(this System.Web.WebPages.Html.HtmlHelper html) {
            return ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).Html;
        }
    }


    public static class NewLabelExtensions {
        public static MvcHtmlString IntraLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression) {
            return LabelFor(html, expression, new RouteValueDictionary(new { @class = "intralabel" }));
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes) {
            return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes) {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText)) {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.SetInnerText(labelText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

    }


    public static class GeneralHTMLExtensions {
        public static IHtmlString URLEncode(this HtmlHelper htmlHelper, string theURL) {
            return MvcHtmlString.Create(HttpUtility.UrlEncode(theURL));
        }

        public static IHtmlString ResultadoBusqueda(this HtmlHelper htmlHelper, IList Lista, int CantidadPorPagina) {
            TagBuilder tag = new TagBuilder("div");
            tag.Attributes.Add("class", "resultadobusqueda");
            if (Lista.Count == 0) {
                tag.SetInnerText("No se encontraron registros");
            }
            else if (Lista.Count == CantidadPorPagina) {
                tag.InnerHtml = "Se encontraron más de " + CantidadPorPagina.ToString() + " registros. <b>Solo se muestran los primeros " + CantidadPorPagina.ToString() + "</b>.";
            }
            else {
                tag.SetInnerText("Se encontraron " + Lista.Count.ToString() + " registros.");
            }
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static IHtmlString ResultadoBusquedaUltimos(this HtmlHelper htmlHelper, IList Lista, int CantidadPorPagina) {
            TagBuilder tag = new TagBuilder("div");
            tag.Attributes.Add("class", "resultadobusqueda");
            if (Lista.Count == 0) {
                tag.SetInnerText("No se encontraron registros");
            }
            else if (Lista.Count == CantidadPorPagina) {
                tag.InnerHtml = "Se encontraron más de " + CantidadPorPagina.ToString() + " registros. <b>Solo se muestran los últimos " + CantidadPorPagina.ToString() + "</b>.";
            }
            else {
                tag.SetInnerText("Se encontraron " + Lista.Count.ToString() + " registros.");
            }
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        
        public static IHtmlString AgregarLineaTr(this HtmlHelper htmlHelper, int RowSpan, string SenseInputSufix = "") {
            return MvcHtmlString.Create(string.Format("<tr class='agregarLineaRow'><td colspan='{0}'><span class='agregarLinea' data-senseinputsufix='" + SenseInputSufix + "'><i class='icon-plus-2'></i> Agregar linea</span></td></tr>", RowSpan));
        }

    }

}