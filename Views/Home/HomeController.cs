using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;

namespace Site.Controllers {
    public class HomeController : ApplicationController {
		[Authorize]
        public ActionResult Index() {
            var VM = new HomeViewModel();
            return View(VM);
        }

        public ActionResult About() {
            return View();
        }

        [NoCache]
        public ActionResult BookmarkPage(string Path, string Titulo) {
            var db = DbHelper.CurrentDb();
            var Bookmarked = db.SingleOrDefault<bool?>("Select IsBookmarked from PaginaVisita where UsuarioID = @0 and Path = @1", Sitio.Usuario.UsuarioID, Path);

            if (Bookmarked == null) {
                var p = new PaginaVisita();
                p.Path = Path;
                p.Titulo = Titulo;
                p.UsuarioID = Sitio.Usuario.UsuarioID;
                p.Cantidad = 0;
                p.IsBookmarked = true;
                p.Orden = 9999;
                p.LastVisited = DateTime.Now;
                db.Save(p);

                Bookmarked = true;
            }
            else {
                Bookmarked = !Bookmarked;
                db.Execute("Update PaginaVisita Set IsBookmarked = @0, Titulo = @1 where UsuarioID = @2 and Path = @3", Bookmarked, Titulo, Sitio.Usuario.UsuarioID, Path);
            }
            return Content(Bookmarked ?? true ? "BOOKMARKED" : "UNBOOKMARKED");
        }



    }
}

