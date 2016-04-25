using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;

namespace Site.Controllers {

    [OutputCache(Location = OutputCacheLocation.None)]
    public class ErrorController : ApplicationController {

        public ActionResult Index() {
            ViewBag.Title = "Error";
            ViewBag.Description = "Hubo un error grave en el sitio. Nuestro Webmaster ya fue advertido del error y será solucionado a la brevedad. Por favor, reintente en unos minutos.";
            if (Request.IsAjaxRequest()) {
                return Content("Error: " + ViewBag.Description);
            }
            else {
                return View("Error");
            }

        }


        public ActionResult HttpError404(string ErrorDescription) {
            Response.StatusCode = 404;
            ViewBag.Title = "Página no encontrada (404)";
            ViewBag.Description = !ErrorDescription.IsEmpty() ? ErrorDescription : "La página o ítem ya no se encuentra en nuestro sitio o en la base de datos";
            if (Request.IsAjaxRequest()) {
                return Content("Error: " + ViewBag.Description);
            }
            else {
                return View("Error");
            }
        }


        public ActionResult HttpError500(string ErrorDescription) {
            Response.StatusCode = 500;
            ViewBag.Title = "Error general del sitio (500)";
            ViewBag.Description = "Hubo un error grave en el sitio. Nuestro Webmaster ya fue advertido del error y será solucionado a la brevedad. Por favor, reintente en unos minutos.";
            if (Request.IsAjaxRequest()) {
                return Content("Error: " + ViewBag.Description);
            }
            else {
                return View("Error");
            }
        }

        public ActionResult General() {
            ViewBag.Title = "Error general del sitio";
            ViewBag.Description = "Hubo un error grave en el sitio. Nuestro Webmaster ya fue advertido del error y será solucionado a la brevedad. Por favor, reintente en unos minutos.";
            if (Request.IsAjaxRequest()) {
                return Content("Error: " + ViewBag.Description);
            }
            else {
                return View("Error");
            }
        }

    }
}
