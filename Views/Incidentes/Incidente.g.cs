using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;

namespace Site.Models {
    [TableName("Incidente")]
    [PrimaryKey("IncidenteID")]
    [ExplicitColumns]
    public class Incidente {

        public const string UploadFolder = "/content/subidos/incidente/";

        [PetaPoco.Column("IncidenteID")]
        public int IncidenteID { get; set; }

        [PetaPoco.Column("Titulo")]
        [Display(Name = "Titulo")]
        [Required(ErrorMessage = "Requerido")]
        public String Titulo { get; set; }

        [PetaPoco.Column("Notas")]
        [Display(Name = "Notas")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public String Notas { get; set; }

        [PetaPoco.Column("Fecha")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; }

        [PetaPoco.Column("US_ID")]
        [Display(Name = "Usuario")]
        public int? US_ID { get; set; }

        [ResultColumn]
        public string Usuario { get; set; }

        [ResultColumn]
        public string AvatarChico { get; set; }

        [PetaPoco.Column("Tipo")]
        [Required(ErrorMessage = "Tipo")]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; }
        
        [PetaPoco.Column("Cerrado")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Cerrado")]
        public Boolean Cerrado { get; set; }


        [PetaPoco.Column("ResueltoDev")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "ResueltoDev")]
        public Boolean ResueltoDev { get; set; }


        [ResultColumn]
        public int CantArchivos { get; set; }

        [ResultColumn]
        public int CantComentarios { get; set; }


        public string EstadoCssClass {
            get { return (this.Cerrado ? "cerrado" : "abierto"); }
        }

        public string Estado {
            get { return (this.Cerrado ? "Cerrado" : "Abierto"); }
        }

        public HtmlString TipoIcon {
            get { 
                if (this.Tipo == "Bug") {
                    return new HtmlString( "<i class='icon-bug'></i>");
                }
                return new HtmlString("<i class='icon-gift'></i>");
            }
        }

        public bool PuedeModificar {
            get {
                return (this.US_ID == Sitio.Usuario.UsuarioID || Seguridad.CanAccess((int)Seguridad.Permisos.Modificar_Cualquier_Incidente));
            }
        }


        public bool IsValid(ModelStateDictionary ModelState) {
            //if (DbHelper.CurrentDb().ExecuteScalar<int>("SELECT count(*) From Usuario where US_ID <> @0 and Us_Login = @1", this.US_ID, this.Login) > 0) {
            //    ModelState.AddModelError("All", "El nombre de usuario ya fue usado por otra persona");
            //return false;
            //}
            return true;
        }


        public static Incidente SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE Incidente.IncidenteID = @0", id);
            return DbHelper.CurrentDb().SingleOrDefault<Incidente>(sql);
        }

        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
            sql.AppendSelectTop(TopN);
            sql.Append("Incidente.*, Usuario.Nombre as Usuario, Usuario.AvatarChico, ");
            sql.Append("(SELECT Count(*) from IncidenteComentario where IncidenteComentario.IncidenteID = Incidente.IncidenteID and IncidenteComentario.Mensaje is not null and IsDeleted = 0) as CantComentarios,");
            sql.Append("(SELECT Count(*) from IncidenteComentario where IncidenteComentario.IncidenteID = Incidente.IncidenteID and IncidenteComentario.Archivo is not null and IsDeleted = 0) as CantArchivos");
            sql.Append("FROM Incidente");
            sql.Append("INNER JOIN Usuario ON Incidente.US_ID = Usuario.UsuarioID");
            return sql;
        }

        private List<IncidenteComentario> _Timeline;
        public List<IncidenteComentario> Timeline {
            get {
                if (_Timeline == null) {
                    var db = DbHelper.CurrentDb();
                    var sql = IncidenteComentario.BaseQuery();
                    sql.Append("WHERE IncidenteComentario.IncidenteID = @0", this.IncidenteID);
                    sql.Append("AND IsDeleted = 0");
                    _Timeline = db.Fetch<IncidenteComentario>(sql);
                }
                return _Timeline;
            }
        }


        private List<Usuario> _Participantes;
        public List<Usuario> Participantes {
            get {
                if (_Participantes == null) {
                    var db = DbHelper.CurrentDb();
                    var sql = Models.Usuario.BaseQuery();
                    sql.Append("WHERE Usuario.UsuarioID IN (SELECT US_ID from IncidenteSubscripcion where IncidenteID = @0)", this.IncidenteID);
                    _Participantes = db.Fetch<Usuario>(sql);
                }
                return _Participantes;
            }
        }


        //private List<IncidenteComentario> _Comentarios;
        //public List<IncidenteComentario> Comentarios {
        //    get {
        //        if (_Comentarios == null) {
        //            var db = DbHelper.CurrentDb();
        //            var sql = IncidenteComentario.BaseQuery();
        //            sql.Append("WHERE IncidenteComentario.IncidenteID = @0", this.IncidenteID);
        //            sql.Append("AND Mensaje is not null");
        //            sql.Append("AND IsDeleted = 0");
        //            _Comentarios = db.Fetch<IncidenteComentario>(sql);
        //        }
        //        return _Comentarios;
        //    }
        //}

        //private List<IncidenteComentario> _Archivos;
        //public List<IncidenteComentario> Archivos {
        //    get {
        //        if (_Archivos == null) {
        //            var db = DbHelper.CurrentDb();
        //            var sql = IncidenteComentario.BaseQuery();
        //            sql.Append("WHERE IncidenteArchivo.IncidenteID = @0", this.IncidenteID);
        //            sql.Append("AND Archivo is not null");
        //            sql.Append("AND IsDeleted = 0");
        //            _Archivos = db.Fetch<IncidenteComentario>(sql);
        //        }
        //        return _Archivos;
        //    }
        //}
        
    }
}
