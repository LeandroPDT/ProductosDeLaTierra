
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;

namespace Site.Models {
    [TableName("IncidenteComentario")]
    [PrimaryKey("IncidenteComentarioID")]
    [ExplicitColumns]
    public class IncidenteComentario {
        [PetaPoco.Column("IncidenteComentarioID")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "IncidenteComentarioID")]
        public int IncidenteComentarioID { get; set; }

        [PetaPoco.Column("IncidenteID")]
        [Display(Name = "IncidenteID")]
        public int? IncidenteID { get; set; }

        [ResultColumn]
        [Display(Name = "Incidente")]
        public string Incidente { get; set; }


        [PetaPoco.Column("US_ID")]
        [Display(Name = "US_ID")]
        public int? US_ID { get; set; }

        [ResultColumn]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [ResultColumn]
        public string AvatarChico { get; set; }

        [PetaPoco.Column("Fecha")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; }

        [PetaPoco.Column("Mensaje")]
        [Display(Name = "Mensaje")]
        [DataType(DataType.MultilineText)]
        public String Mensaje { get; set; }

        [PetaPoco.Column("Archivo")]
        [Display(Name = "Archivo")]
        public String Archivo { get; set; }

        [PetaPoco.Column("ArchivoNombre")]
        [Display(Name = "ArchivoNombre")]
        public String ArchivoNombre { get; set; }

        [PetaPoco.Column("Status")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Status")]
        public String Status { get; set; }

        
        [PetaPoco.Column("IsDeleted")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "IsDeleted")]
        public Boolean IsDeleted { get; set; }

        public bool IsValid(ModelStateDictionary ModelState) {
            //if (DbHelper.CurrentDb().ExecuteScalar<int>("SELECT count(*) From Usuario where US_ID <> @0 and Us_Login = @1", this.US_ID, this.Login) > 0) {
            //    ModelState.AddModelError("All", "El nombre de usuario ya fue usado por otra persona");
            //return false;
            //}
            return true;
        }

        public static IncidenteComentario SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE IncidenteComentario.IncidenteComentarioID = @0", id);
            return DbHelper.CurrentDb().SingleOrDefault<IncidenteComentario>(sql);
        }

        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
            sql.AppendSelectTop(TopN);
            sql.Append("IncidenteComentario.*, Usuario.Nombre as Usuario, Usuario.AvatarChico");
            sql.Append("FROM IncidenteComentario");
            sql.Append("INNER JOIN Usuario ON IncidenteComentario.US_ID = Usuario.UsuarioID");
            return sql;
        }


        public string URL {
            get { return Site.Models.Incidente.UploadFolder + this.Archivo; }
        }

        public string CssClass {
            get {
                switch (System.IO.Path.GetExtension(this.Archivo).ToLower().Trim()) {
                    case ".xls":
                    case ".xlsx":
                        return "excel";
                    case ".pdf":
                        return "pdf";
                    case ".doc":
                    case ".docx":
                        return "word";
                    case ".mp4":
                        return "video";
                    default:
                        return "archivo";
                }
            }
        }

        public string DomID {
            get {
                switch (System.IO.Path.GetExtension(this.Archivo).ToLower().Trim()) {
                    case ".mp4":
                        return "videoopener" + this.IncidenteComentarioID.ToString();
                    default:
                        return "";
                }
            }
        }

        public bool EsImagen {
            get {
                if (this.Archivo == null) return false;
                string ext = (System.IO.Path.GetExtension(this.Archivo).ToLower().Trim());
                return (ext == ".jpg" || ext == ".gif" || ext == ".png");
            }
        }


        public string EstadoCssClass {
            get { return (this.Status == "cerrado" ? "cerrado" : "abierto"); }
        }

        public string Estado {
            get { return (this.Status == "cerrado" ? "Cerrado" : "Reabierto"); }
        }

        public bool PuedeModificar {
            get {
                return (this.US_ID == Sitio.Usuario.UsuarioID || Seguridad.CanAccess((int)Seguridad.Permisos.Modificar_Cualquier_Incidente));
            }
        }
    
    }
}
