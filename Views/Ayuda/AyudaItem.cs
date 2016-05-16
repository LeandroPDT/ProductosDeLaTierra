
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;

namespace Site.Models {
    [TableName("AyudaItem")]
    [PrimaryKey("AyudaItemID")]
    [ExplicitColumns]
    public class AyudaItem {
        [PetaPoco.Column("AyudaItemID")]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "AyudaItemID")]
        public int AyudaItemID { get; set; }

        [PetaPoco.Column("AyudaID")]
        [Display(Name = "AyudaID")]
        public int? AyudaID { get; set; }

        [ResultColumn]
        [Display(Name = "Ayuda")]
        public string Ayuda { get; set; }


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

        public bool IsValid(ModelStateDictionary ModelState) {
            return true;
        }

        public static AyudaItem SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE AyudaItem.AyudaItemID = @0", id);
            return DbHelper.CurrentDb().SingleOrDefault<AyudaItem>(sql);
        }

        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
            sql.AppendSelectTop(TopN);
            sql.Append("AyudaItem.*, Usuario.Nombre as Usuario, Usuario.AvatarChico");
            sql.Append("FROM AyudaItem");
            sql.Append("INNER JOIN Usuario ON AyudaItem.US_ID = Usuario.UsuarioID");
            return sql;
        }


        public string URL {
            get { return Site.Models.Ayuda.UploadFolder + this.Archivo; }
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
                        return "videoopener" + this.AyudaItemID.ToString();
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


        public bool PuedeModificar {
            get {
                return (this.US_ID == Sitio.Usuario.UsuarioID || Seguridad.CanAccess((int)Seguridad.Permisos.ModificarAyuda));
            }
        }
    
    }
}
