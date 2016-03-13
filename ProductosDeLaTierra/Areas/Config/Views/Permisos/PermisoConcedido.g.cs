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
    [TableName("PermisoConcedido")]
    [PrimaryKey("PermisoConcedidoID")]
    [ExplicitColumns]
    public class PermisoConcedido {
        [PetaPoco.Column("PermisoConcedidoID")]
        public int PermisoConcedidoID { get; set; }
        
        [PetaPoco.Column("PermisoID")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "Permiso")]
		public int PermisoID { get; set; }

		[ResultColumn]
		public string Permiso { get; set; }

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

		[PetaPoco.Column("PuedeEntrar")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "PuedeEntrar")]
		public bool PuedeEntrar { get; set; }

		[PetaPoco.Column("PuedeBorrar")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "PuedeBorrar")]
        public bool PuedeBorrar { get; set; }

		[PetaPoco.Column("PuedeEditar")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "PuedeEditar")]
        public bool PuedeEditar { get; set; }

        public bool IsValid(ModelStateDictionary ModelState) {
            return true;
        }
        
		public static PermisoConcedido SingleOrDefault(int id) {
		    var sql = BaseQuery();
            sql.Append("WHERE PermisoConcedido.PermisoConcedidoID = @0", id);
		    return DbHelper.CurrentDb().SingleOrDefault<PermisoConcedido>(sql);
		}
		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
            sql.Append("PermisoConcedido.*, Rol.Nombre as Rol, Usuario.Nombre as Usuario, Permiso.Nombre as Permiso");
		    sql.Append("FROM PermisoConcedido");
            sql.Append("    INNER JOIN Permiso ON PermisoConcedido.PermisoID = Permiso.PermisoID");
		    sql.Append("    LEFT JOIN Rol ON PermisoConcedido.RolID = Rol.RolID");
		    sql.Append("    LEFT JOIN usuario ON PermisoConcedido.UsuarioID = usuario.UsuarioID");
		    return sql;
		}

	}
}
