using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PetaPoco;
using System.Web.Mvc;


namespace Site.Models {
    [TableName("Config")]
    [ExplicitColumns]
    public class Config {
        [PetaPoco.Column("Aspecto")]
        public String Aspecto { get; set; }

        [PetaPoco.Column("Valor")]
        public String Valor { get; set; }

        public bool IsValid(ModelStateDictionary ModelState) {
            return true;
        }

        public static Config SingleOrDefault(string Aspecto) {
            var sql = BaseQuery();
            sql.Append("WHERE Config.Aspecto = @0", Aspecto);
            return DbHelper.CurrentDb().SingleOrDefault<Config>(sql);
        }

        public static PetaPoco.Sql BaseQuery(int TopN = 0) {
            var sql = PetaPoco.Sql.Builder;
            sql.AppendSelectTop(TopN);
            sql.Append("Config.*");
            sql.Append("FROM Config");
            return sql;
        }

        public static decimal Get(string Aspecto, decimal DefaultValue) {
            var c = Config.SingleOrDefault(Aspecto);
            if (c == null || string.IsNullOrEmpty(c.Valor)) {
                return DefaultValue;
            }
            return c.Valor.TryCDec();
        }


        public static int Get(string Aspecto, int DefaultValue) {
            string ValorAsTexto = Get(Aspecto, DefaultValue.ToString());
            return ValorAsTexto.TryCInt();
        }

        public static DateTime Get(string Aspecto, DateTime DefaultValue) {
            System.DateTime retval = default(System.DateTime);
            string ValorAsTexto = Get(Aspecto, "nodate");
            try {
                retval = ValorAsTexto.ISOToDate();
            }
            catch {
                //no lo pude convertir, devuelvo el defaultValue
                retval = DefaultValue;
            }

            //devuelvo el valor
            return retval;

        }

        public static string Get(string Aspecto, string DefaultValue) {
            var c = Config.SingleOrDefault(Aspecto);
            if (c == null || string.IsNullOrEmpty(c.Valor)) {
                return DefaultValue;
            }
            return c.Valor;
        }

        public static void Save(string Aspecto, DateTime Value) {
            Save(Aspecto, Value.ToISO());
        }

        public static void Save(string Aspecto, int Value) {
            Save(Aspecto, Value.ToString());
        }

        public static void Save(string Aspecto, decimal Value) {
            Save(Aspecto, Value.ToString("N4"));
        }

        public static void Save(string Aspecto, string Value) {
            var db = DbHelper.CurrentDb();
            var c = Config.SingleOrDefault(Aspecto);
            if (c == null) {
                db.Execute("INSERT INTO Config (Aspecto, Valor) VALUES (@0, @1)", Aspecto, Value);
            }
            else if (c.Valor != Value) {
                db.Execute("UPDATE Config Set valor = @1 Where Aspecto = @0", Aspecto, Value);
            }
        }


        public static class Current {
            public static int CuentaCorrienteProveedores {
                get {
                    return 1;
                }
                set {
                    
                }
            }
            public static decimal PorcIVA {
                get {
                    return CacheHelper.Get<decimal>("Config.PorcIVA", 30, () => Config.Get("PorcIVA", 21m));
                }
                set {
                    Config.Save("PorcIVA", value);
                }
            }

            public static int CuentaIVACompras {
                get {
                    return CacheHelper.Get<int>("Config.CIvaCompras", 30, () => Config.Get("CuentaIVACompras", 3));
                }
                set {
                    Config.Save("CuentaIVACompras", value);
                }
            }

            public static int CuentaIVAVentas {
                get {
                    return CacheHelper.Get<int>("Config.CIvaVentas", 30, () => Config.Get("CuentaIVAVentas", 4));
                }
                set {
                    Config.Save("CuentaIVAVentas", value);
                }
            }

            public static int DiasMaximosAsiento {
                get {
                    return CacheHelper.Get<int>("Asiento.DiasMaximos", 30, () => Config.Get("DiasMaximosAsiento", 365));
                }
                set {
                    Config.Save("DiasMaximosAsiento", value);
                }
            }



                
        }




    }
}
