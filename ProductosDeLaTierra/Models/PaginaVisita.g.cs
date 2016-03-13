using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;
using System.Web.WebPages;
using BizLibMVC;

namespace Site.Models {
    [TableName("PaginaVisita")]
    [PrimaryKey("PaginaVisitaID")]
    [ExplicitColumns]
    public class PaginaVisita {
		[PetaPoco.Column("PaginaVisitaID")]
		public int PaginaVisitaID { get; set; }

		[PetaPoco.Column("UsuarioID")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "UsuarioID")]
		public int UsuarioID { get; set; }

		[PetaPoco.Column("Path")]
		[Display(Name = "Path")]
		[StringLength(250)]
		public String Path { get; set; }

		[PetaPoco.Column("Titulo")]
		[Display(Name = "Titulo")]
		[StringLength(100)]
		public String Titulo { get; set; }

		[PetaPoco.Column("Cantidad")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "Cantidad")]
		public int Cantidad { get; set; }

		[PetaPoco.Column("IsBookmarked")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "IsBookmarked")]
		public Boolean IsBookmarked { get; set; }

		[PetaPoco.Column("Orden")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "Orden")]
		public int Orden { get; set; }

        [PetaPoco.Column("LastVisited")]
        public DateTime LastVisited { get; set; }


        public bool IsValid(ModelStateDictionary ModelState) {
            //if (DbHelper.CurrentDb().ExecuteScalar<int>("SELECT count(*) From Usuario where UsuarioID <> @0 and Us_Login = @1", this.UsuarioID, this.Login) > 0) {
            //    ModelState.AddModelError("All", "El nombre de usuario ya fue usado por otra persona");
            //return false;
            //}
            return true;
        }
        
		public static PaginaVisita SingleOrDefault(int id) {
		    var sql = BaseQuery();
		    sql.Append("WHERE PaginaVisita.PaginaVisitaID = @0", id);
		    return DbHelper.CurrentDb().SingleOrDefault<PaginaVisita>(sql);
		}
		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
		    sql.Append("PaginaVisita.*");
		    sql.Append("FROM PaginaVisita");
		    return sql;
		}

	}
}
