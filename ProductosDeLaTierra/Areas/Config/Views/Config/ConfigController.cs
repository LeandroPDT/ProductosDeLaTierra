using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;
using BizLibMVC;
using System.Web.WebPages;

namespace Site.Areas.Config.Controllers {
    [Authorize]
    public class ConfigController : ApplicationController {
        public ActionResult Index() {
            return View();
        }
    }
}

