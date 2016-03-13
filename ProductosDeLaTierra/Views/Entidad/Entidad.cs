using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BizLibMVC;
using PetaPoco;

namespace Site.Models {
    public class Entidad {
        public string Nombre { get; set; }
        public int ID { get; set; }

        private IEnumerable<ArchivoAdjunto> _ArchivosAdjuntos;

        public string UniqueID {
            get {
                return "ent-" + Nombre + "-" + ID.ToString();
            }
        }


        public IEnumerable<ArchivoAdjunto> ArchivosAdjuntos {
            get {
                if (_ArchivosAdjuntos == null) {
                    _ArchivosAdjuntos = DbHelper.CurrentDb().Fetch<ArchivoAdjunto>("SELECT * From ArchivoAdjunto where Entidad = @0 AND ID = @1", Nombre, ID);
                }
                return _ArchivosAdjuntos;
            }
            set { _ArchivosAdjuntos = value; }
        }

        public static Entidad GetFromModel(object rec) {
            var pd = Database.PocoData.ForType(rec.GetType());
            var pc = pd.Columns[pd.TableInfo.PrimaryKey];

            var e = new Entidad();
            e.Nombre = pd.TableInfo.TableName;
            e.ID = (int)pc.GetValue(rec);

            return e;
        }



    }
}