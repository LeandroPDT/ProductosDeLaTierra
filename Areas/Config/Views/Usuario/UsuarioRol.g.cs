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
    [TableName("UsuarioRol")]
    [PrimaryKey("UsuarioRolID")]
    [ExplicitColumns]
    public class UsuarioRol {
        [PetaPoco.Column("UsuarioRolID")]
        public int UsuarioRolID { get; set; }
        
		[PetaPoco.Column("RolID")]
		[Display(Name = "Rol")]
		public int? RolID { get; set; }

		[ResultColumn]
		public string Rol { get; set; }

		[PetaPoco.Column("UsuarioID")]
		[Display(Name = "Usuario")]
		public int? UsuarioID { get; set; }

		[ResultColumn]
		public string Usuario { get; set; }


        public bool IsValid(ModelStateDictionary ModelState) {
            return true;
        }
        
		public static UsuarioRol SingleOrDefault(int id) {
		    var sql = BaseQuery();
            sql.Append("WHERE UsuarioRol.UsuarioRolID = @0", id);
		    return DbHelper.CurrentDb().SingleOrDefault<UsuarioRol>(sql);
		}
		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
            sql.Append("UsuarioRol.*, Rol.Nombre as Rol, Usuario.Nombre as Usuario");
		    sql.Append("FROM UsuarioRol");
		    sql.Append("    LEFT JOIN Rol ON UsuarioRol.RolID = Rol.RolID");
		    sql.Append("    LEFT JOIN usuario ON UsuarioRol.UsuarioID = usuario.UsuarioID");
		    return sql;
		}

	}
}
