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
    [TableName("Permiso")]
    [PrimaryKey("PermisoID")]
    [ExplicitColumns]
    public class Permiso {
		[PetaPoco.Column("PermisoID")]
		public int PermisoID { get; set; }

		[PetaPoco.Column("Nombre")]
		[Display(Name = "Nombre")]
		[StringLength(100)]
		public String Nombre { get; set; }


        [PetaPoco.Column("Notas")]
		[Display(Name = "Notas")]
		public String Notas { get; set; }

		[PetaPoco.Column("EsABM")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "EsABM")]
		public Boolean EsABM { get; set; }


        [PetaPoco.Column("Activo")]
        [Display(Name = "Activo")]
        public Boolean Activo { get; set; }

        [ResultColumn]
        public int CantRoles { get; set; }

        [ResultColumn]
        public int CantUsuarios { get; set; }


        public bool IsValid(ModelStateDictionary ModelState) {
            //if (DbHelper.CurrentDb().ExecuteScalar<int>("SELECT count(*) From Usuario where UsuarioID <> @0 and Username = @1", this.UsuarioID, this.Login) > 0) {
            //    ModelState.AddModelError("All", "El nombre de usuario ya fue usado por otra persona");
            //return false;
            //}
            return true;
        }
        
		public static Permiso SingleOrDefault(int id) {
		    var sql = BaseQuery();
		    sql.Append("WHERE Permiso.PermisoID = @0", id);
		    return DbHelper.CurrentDb().SingleOrDefault<Permiso>(sql);
		}
		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
		    sql.Append("Permiso.*");
		    sql.Append("FROM Permiso");
		    return sql;
		}

        private IList<PermisoConcedido> _RolesConPermiso;
        public IList<PermisoConcedido> RolesConPermiso {
            get {
                if (_RolesConPermiso == null) {
                    var sql = PermisoConcedido.BaseQuery();
                    sql.Append("WHERE PermisoConcedido.PermisoID = @0", this.PermisoID);
                    sql.Append("AND PermisoConcedido.RolID is not null");
                    sql.Append("ORDER BY Rol.Nombre");
                    _RolesConPermiso = DbHelper.CurrentDb().Fetch<PermisoConcedido>(sql);
                }
                return _RolesConPermiso;
            }
            set {
                _RolesConPermiso = value;
            }
        }

        public void InitRolesConPermiso(int minSlots = 5, int minFreeSlots = 2) {
            //me aseguro que la minimamente la cantidad que quiero libre
            for (int i = RolesConPermiso.Count(); i < minSlots; i++) {
                RolesConPermiso.Add(new PermisoConcedido());
                minFreeSlots--;
            }
            //me aseguro que haya algunos lugar libres
            for (int i = 0; i < minFreeSlots; i++) {
                RolesConPermiso.Add(new PermisoConcedido());
            }
        }


        private IList<PermisoConcedido> _UsuariosConPermiso;
        public IList<PermisoConcedido> UsuariosConPermiso {
            get {
                if (_UsuariosConPermiso == null) {
                    var sql = PermisoConcedido.BaseQuery();
                    sql.Append("WHERE PermisoConcedido.PermisoID = @0", this.PermisoID);
                    sql.Append("AND PermisoConcedido.UsuarioID is not null");
                    sql.Append("ORDER BY Usuario.Nombre");
                    _UsuariosConPermiso = DbHelper.CurrentDb().Fetch<PermisoConcedido>(sql);
                }
                return _UsuariosConPermiso;
            }
            set {
                _UsuariosConPermiso = value;
            }
        }

        public void InitUsuariosConPermiso(int minSlots = 5, int minFreeSlots = 2) {
            //me aseguro que la minimamente la cantidad que quiero libre
            for (int i = UsuariosConPermiso.Count(); i < minSlots; i++) {
                UsuariosConPermiso.Add(new PermisoConcedido());
                minFreeSlots--;
            }
            //me aseguro que haya algunos lugar libres
            for (int i = 0; i < minFreeSlots; i++) {
                UsuariosConPermiso.Add(new PermisoConcedido());
            }
        }



	}
}
