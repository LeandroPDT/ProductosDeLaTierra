using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Site.Models {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HandleAjaxError : ActionFilterAttribute, IExceptionFilter {

        public void OnException(ExceptionContext filterContext) {

            string message = "";

            if (!filterContext.HttpContext.Request.IsAjaxRequest()) return;

            var modelState = (filterContext.Controller as Controller).ModelState;
            if (!modelState.IsValid) {
                var errors = modelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new {
                        x.Key,
                        x.Value.Errors
                    });
            }
            else {
                message = filterContext.Exception.Message;
            }

            filterContext.Result = AjaxError(message, filterContext);

            //Let the system know that the exception has been handled
            filterContext.ExceptionHandled = true;
        }

        protected ContentResult AjaxError(string message, ExceptionContext filterContext) {
            //If message is null or empty, then fill with generic message
            if (String.IsNullOrEmpty(message))
                message = "Hubo un error indeterminado";

            //Set the response status code to 500
            filterContext.HttpContext.Response.StatusCode = 200;

            //Needed for IIS7.0
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            return new ContentResult {
                Content = "ERROR: " + message,
                ContentEncoding = System.Text.Encoding.UTF8
            };
        }
    }
}