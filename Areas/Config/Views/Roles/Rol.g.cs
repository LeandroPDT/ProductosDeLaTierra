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
    [TableName("Rol")]
    [PrimaryKey("RolID")]
    [ExplicitColumns]
    public class Rol {
		[PetaPoco.Column("RolID")]
		public int RolID { get; set; }

		[PetaPoco.Column("Nombre")]
		[Display(Name = "Nombre")]
		[StringLength(50)]
		public String Nombre { get; set; }

		[PetaPoco.Column("Notas")]
		[Display(Name = "Notas")]
		[StringLength(100)]
		public String Descrip { get; set; }

        public override string ToString() {
            return (RolID.IsEmpty() ? "Nuevo Rol" : "Rol " + this.Nombre);
        }

        public bool IsValid(ModelStateDictionary ModelState) {
            return true;
        }

        private IList<IDNombrePar> _Items;
        public IList<IDNombrePar> Items {
            get {
                if (_Items == null) {
                    var sql = PetaPoco.Sql.Builder;
                    sql.Append("SELECT Usuario.UsuarioID as ID, Usuario.Nombre");
                    sql.Append("FROM Usuario");
                    sql.Append("INNER JOIN usuarioRol on usuario.UsuarioID = usuarioRol.UsuarioID");
                    sql.Append("where UsuarioRol.RolID = @0", this.RolID);
                    sql.Append("ORDER BY usuario.Nombre");
                    _Items = DbHelper.CurrentDb().Fetch<IDNombrePar>(sql);
                }
                return _Items;
            }
            set {
                _Items = value;
            }
        }

        public void InitItems(int minSlots = 5, int minFreeSlots = 2) {
            //me aseguro que la minimamente la cantidad que quiero libre
            for (int i = Items.Count(); i < minSlots; i++) {
                Items.Add(new IDNombrePar());
                minFreeSlots--;
            }
            //me aseguro que haya algunos lugar libres
            for (int i = 0; i < minFreeSlots; i++) {
                Items.Add(new IDNombrePar());
            }
        }
                
		public static Rol SingleOrDefault(int id) {
		    var sql = BaseQuery();
		    sql.Append("WHERE Rol.RolID = @0", id);
		    return DbHelper.CurrentDb().SingleOrDefault<Rol>(sql);
		}
		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
		    sql.Append("Rol.*");
		    sql.Append("FROM Rol");
		    return sql;
		}

	}
}
