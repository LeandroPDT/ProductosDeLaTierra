using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web;

namespace Site.Models {
    public class CustomAuthorizeBorrarAttribute : AuthorizeAttribute {
        // the "new" must be used here because we are overriding
        // the Roles property on the underlying class
        public new Seguridad.Permisos Roles;


        public override void OnAuthorization(AuthorizationContext filterContext) {
            base.OnAuthorization(filterContext);

            if (filterContext == null) {
                throw new ArgumentNullException("filterContext");
            }

            if (Sitio.Usuario == null || Sitio.Usuario.UsuarioID == 0) {
                filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?returnUrl=" + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
                return;
            }

            if (Roles != 0 && !Seguridad.CanDelete((int)Roles)) {
                filterContext.Result = new ViewResult { ViewName = filterContext.HttpContext.Request.IsAjaxRequest() ? "~/Views/Shared/_AccessDeniedAjax.cshtml" : "~/Views/Shared/AccessDenied.cshtml" };
            }

        }



    }

}
