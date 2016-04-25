using System.Web.Mvc;

namespace Site.Areas.Eventos {
    public class EventoAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Eventos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Eventos_default",
                "Eventos/{controller}/{action}/{type}/{id}",
                new { controller = "Evento", action = "Index", type = UrlParameter.Optional,id = UrlParameter.Optional },
                new string[] { "Site.Areas.Eventos.Controllers" }
            );
        }
    }
}
