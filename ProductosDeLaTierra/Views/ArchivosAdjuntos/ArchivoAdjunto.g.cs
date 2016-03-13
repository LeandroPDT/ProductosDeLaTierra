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
    [TableName("ArchivoAdjunto")]
    [PrimaryKey("ArchivoAdjuntoID")]
    [ExplicitColumns]
    public class ArchivoAdjunto {
		[PetaPoco.Column("ArchivoAdjuntoID")]
		public int ArchivoAdjuntoID { get; set; }

		[PetaPoco.Column("Fecha")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "Fecha")]
		[DataType(DataType.DateTime)]
		public DateTime Fecha { get; set; }

		[PetaPoco.Column("ID")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "ID")]
		public int ID { get; set; }

		[PetaPoco.Column("Entidad")]
		[Required(ErrorMessage = "Requerido")]
		[Display(Name = "Entidad")]
		[StringLength(50)]
		public String Entidad { get; set; }

		[PetaPoco.Column("NombreArchivo")]
		[Display(Name = "NombreArchivo")]
		[StringLength(250)]
		public String NombreArchivo { get; set; }

		[PetaPoco.Column("Titulo")]
		[Display(Name = "Titulo")]
		[StringLength(250)]
		public String Titulo { get; set; }

        public string PathAndFile { 
            get {
                return "/Content/Subidos/" + Entidad + "/" + NombreArchivo;
            }
        }
        public string Thumb { 
            get {
                if (EsImagen) { 
                    return Utiles.SaveAsThumbnail(HttpContext.Current.Server.MapPath("/Content/Subidos/" + Entidad + "/"), NombreArchivo, 200, 1000);
                }
                return "";
            } 
        }
        public string PathAndThumb { 
            get {
                if (EsImagen) {
                    return "/Content/Subidos/" + Entidad + "/" + Thumb;
                }
                return "";
            } 
        }

        public Boolean EsImagen {
            get {
                string ext = System.IO.Path.GetExtension(NombreArchivo);
                return (ext == ".png" || ext == ".jpg" || ext == ".gif" || ext == ".jpeg"); 
            }
        }

        public string Icon {
            get {
                string ext = System.IO.Path.GetExtension(NombreArchivo);
                if (ext == ".pdf" ) return "icon-file-pdf";
                if (ext == ".ppt" || ext == ".pptx") return "icon-file-powerpoint";
                if (ext == ".xls" || ext == ".xlsx" ) return "icon-file-excel";
                if (ext == ".doc" || ext == ".docx") return "icon-file-word";
                return "icon-file";
            }
        }
        


        public bool IsValid(ModelStateDictionary ModelState) {
            //if (DbHelper.CurrentDb().ExecuteScalar<int>("SELECT count(*) From Usuario where US_ID <> @0 and Us_Login = @1", this.US_ID, this.Login) > 0) {
            //    ModelState.AddModelError("All", "El nombre de usuario ya fue usado por otra persona");
            //return false;
            //}
            return true;
        }
        
		public static ArchivoAdjunto SingleOrDefault(int id) {
		    var sql = BaseQuery();
		    sql.Append("WHERE ArchivoAdjunto.ArchivoAdjuntoID = @0", id);
		    return DbHelper.CurrentDb().SingleOrDefault<ArchivoAdjunto>(sql);
		}
		public static PetaPoco.Sql BaseQuery(int TopN = 0) {
		    var sql = PetaPoco.Sql.Builder;
		    sql.AppendSelectTop(TopN);
		    sql.Append("ArchivoAdjunto.*");
		    sql.Append("FROM ArchivoAdjunto");
		    return sql;
		}

	}
}
