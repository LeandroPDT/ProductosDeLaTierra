using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;
using System.Web.WebPages;
using DataAnnotationsExtensions;

namespace Site.Models {
    [TableName("Log")]
    [ExplicitColumns]
    public class Log {
		[PetaPoco.Column("Fecha")]
		[Display(Name = "Fecha")]
		[DataType(DataType.DateTime)]
		public DateTime? Fecha { get; set; }

		[PetaPoco.Column("Tipo")]
		[Display(Name = "Tipo")]
		[StringLength(50)]
		public String Tipo { get; set; }


		[PetaPoco.Column("Mensaje")]
		[Display(Name = "Mensaje")]
		[DataType(DataType.MultilineText)]
		public String Mensaje { get; set; }

		[PetaPoco.Column("NombreTabla")]
		[Display(Name = "NombreTabla")]
		[StringLength(100)]
		public String NombreTabla { get; set; }

		[PetaPoco.Column("ID")]
		[Display(Name = "ID")]
		public int? ID { get; set; }

		[PetaPoco.Column("UsuarioID")]
		[Display(Name = "UsuarioID")]
		public int? UsuarioID { get; set; }

        [ResultColumn]
        public String Usuario { get; set; }

        public bool IsValid(ModelStateDictionary ModelState) {
            //if (DbHelper.CurrentDb().ExecuteScalar<int>("SELECT count(*) From EnsayoAro where EnsayoAroID <> @0 and OB_ID = @1 AND Numero = @2", this.EnsayoAroID, this.OB_ID, this.Numero) > 0) {
            //    ModelState.AddModelError("All", "Ya indicó ese numero de Aro para esa obra");
            //    return false;
            //}
            return true;
        }

        public bool CanUpdate(ModelStateDictionary ModelState) {
            return true;
        }

		public static Log SingleOrDefault(int id) {
		    var sql = BaseQuery();
		    sql.Append("WHERE Log.NoAplicable = @0", id);
		    return DbHelper.CurrentDb().SingleOrDefault<Log>(sql);
		}

		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
		    sql.Append("Log.*, Usuario.Nombre as Usuario");
		    sql.Append("FROM Log");
            sql.Append("LEFT JOIN Usuario On Log.UsuarioID = Usuario.UsuarioID");
		    return sql;
		}

	}
}
