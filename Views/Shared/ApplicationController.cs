using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Site.Models;

public abstract class ApplicationController : System.Web.Mvc.Controller
{
    public bool IsAdmin {get; set;}
    public bool EstaLogeado { get; set; }

    public ApplicationController()
    {
        //seteo un par de propiedades comunes necesarios por todos lados
        EstaLogeado = System.Web.HttpContext.Current.Request.IsAuthenticated;
        ViewBag.EstaLogeado = EstaLogeado;
        ViewBag.PuedeModificar = false;

    }

}