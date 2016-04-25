﻿using System.Web.Mvc;

namespace Site.Areas.Config {
    public class ConfigAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Config";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Config_default",
                "Config/{controller}/{action}/{id}",
                new { controller = "Config", action = "Index", id = UrlParameter.Optional },
                new string[] { "Site.Areas.Config.Controllers" }
            );
        }
    }
}
