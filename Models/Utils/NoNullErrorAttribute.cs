using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Site.Models {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NoNullError : ActionFilterAttribute, IExceptionFilter {

        public void OnException(ExceptionContext filterContext) {

            if (filterContext.Exception is NullReferenceException && !Sitio.EsDeveloper()) {
                var result = CreateActionResult(filterContext);
                filterContext.Result = result;

                // Prepare the response code.
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 404;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }

        }

        protected virtual ActionResult CreateActionResult(ExceptionContext filterContext) {
            var ctx = new ControllerContext(filterContext.RequestContext, filterContext.Controller);
            var viewName = "~/Views/Shared/Error.cshtml";
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            var result = new ViewResult {
                ViewName = viewName,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            };
            result.ViewBag.Title = "Registro no encontrado (404)";
            result.ViewBag.Description = "El registro o ítem que esta buscando fue borrado o ya no existe en la base de datos";

            return result;
        }

    }
}