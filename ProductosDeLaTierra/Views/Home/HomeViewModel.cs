using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Site.Models;

namespace Site.Models {
	public class HomeViewModel {

		public List<PaginaVisita> Bookmarked { get; set; }
		public List<PaginaVisita> MostUsed { get; set; }
		public List<PaginaVisita> Recent { get; set; }

		public HomeViewModel() {
			var db = DbHelper.CurrentDb();
			var sql = PaginaVisita.BaseQuery();
			sql.Append("Where PaginaVisita.UsuarioID = @0", Sitio.Usuario.UsuarioID);
			sql.Append("AND IsBookmarked <> 0");
			sql.Append("order by Orden");
			Bookmarked = db.Fetch<PaginaVisita>(sql);

			int extras = 16 - Bookmarked.Count();
			if (extras > 0) {
				sql = PaginaVisita.BaseQuery(extras);
				sql.Append("Where PaginaVisita.UsuarioID = @0", Sitio.Usuario.UsuarioID);
				sql.Append("AND IsBookmarked = 0");
				sql.Append("order by Cantidad DESC");
				MostUsed = db.Fetch<PaginaVisita>(sql);
			}
			else {
				MostUsed = new List<PaginaVisita>();
			}


			sql = PaginaVisita.BaseQuery(12);
			sql.Append("Where PaginaVisita.UsuarioID = @0", Sitio.Usuario.UsuarioID);
			sql.Append("order by LastVisited DESC");
			Recent = db.Fetch<PaginaVisita>(sql);
		}

	}
}