using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;
using PetaPoco;

namespace Site.Controllers {
    public class EntidadController : Controller {
        public ActionResult Toolbox(Entidad e) {
            return PartialView(e);
        }
        public ActionResult GridToolbox(ViewContext VC) {
            var VM = new GridToolboxViewModel();
            VM.Controller = VC.RouteData.DataTokens["controller"].ToString();
            return PartialView(VM);
        }
        public ActionResult Info(int id, string Nombre) {
            return PartialView(DbHelper.CurrentDb().ListLog(Nombre, id));
        }
    }
}
namespace Site.Models {
    public class GridToolboxViewModel {
        public string Controller { get; set; }
    }
}
