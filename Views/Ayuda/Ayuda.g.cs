using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;

namespace Site.Models {
    [TableName("Ayuda")]
    [PrimaryKey("AyudaID")]
    [ExplicitColumns]
    public class Ayuda {

        public const string UploadFolder = "/content/subidos/ayuda/";

        [PetaPoco.Column("AyudaID")]
        public int AyudaID { get; set; }

        [PetaPoco.Column("Codigo")]
        public String Codigo { get; set; }
        
        [PetaPoco.Column("Titulo")]
        [Display(Name = "Titulo")]
        [StringLength(250)]
        public String Titulo { get; set; }

        [PetaPoco.Column("Notas")]
        [Display(Name = "Notas")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public String Notas { get; set; }

        [ResultColumn]
        public int CantArchivos { get; set; }

        [ResultColumn]
        public int CantComentarios { get; set; }

        public bool PuedeModificar {
            get {
                return (Seguridad.CanAccess((int)Seguridad.Permisos.ModificarAyuda));
            }
        }


        public bool IsValid(ModelStateDictionary ModelState) {
            return true;
        }


        public static Ayuda SingleOrDefault(int id) {
            var sql = BaseQuery();
            sql.Append("WHERE Ayuda.AyudaID = @0", id);
            return DbHelper.CurrentDb().SingleOrDefault<Ayuda>(sql);
        }

        public static Ayuda SingleOrDefault(string Codigo) {
            var sql = BaseQuery();
            sql.Append("WHERE Ayuda.Codigo = @0", Codigo);
            return DbHelper.CurrentDb().SingleOrDefault<Ayuda>(sql);
        }


        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
            sql.AppendSelectTop(TopN);
            sql.Append("Ayuda.*, ");
            sql.Append("(SELECT Count(*) from AyudaItem where AyudaItem.AyudaID = Ayuda.AyudaID and AyudaItem.Mensaje is not null) as CantComentarios,");
            sql.Append("(SELECT Count(*) from AyudaItem where AyudaItem.AyudaID = Ayuda.AyudaID and AyudaItem.Archivo is not null) as CantArchivos");
            sql.Append("FROM Ayuda");
            return sql;
        }

        private List<AyudaItem> _Timeline;
        public List<AyudaItem> Timeline {
            get {
                if (_Timeline == null) {
                    var db = DbHelper.CurrentDb();
                    var sql = AyudaItem.BaseQuery();
                    sql.Append("WHERE AyudaItem.AyudaID = @0", this.AyudaID);
                    _Timeline = db.Fetch<AyudaItem>(sql);
                }
                return _Timeline;
            }
        }

        private List<AyudaItem> _Archivos;
        public List<AyudaItem> Archivos {
            get {
                if (_Archivos == null) {
                    var db = DbHelper.CurrentDb();
                    var sql = AyudaItem.BaseQuery();
                    sql.Append("WHERE AyudaItem.AyudaID = @0", this.AyudaID);
                    sql.Append("AND AyudaItem.Archivo IS NOT NULL");
                    _Archivos = db.Fetch<AyudaItem>(sql);
                }
                return _Archivos;
            }
        }

        private List<AyudaItem> _Comentarios;
        public List<AyudaItem> Comentarios {
            get {
                if (_Comentarios == null) {
                    var db = DbHelper.CurrentDb();
                    var sql = AyudaItem.BaseQuery();
                    sql.Append("WHERE AyudaItem.AyudaID = @0", this.AyudaID);
                    sql.Append("AND AyudaItem.Mensaje IS NOT NULL");
                    _Comentarios = db.Fetch<AyudaItem>(sql);
                }
                return _Comentarios;
            }
        }

        
    }
}
