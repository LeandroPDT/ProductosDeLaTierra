using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Text;
using System.Web.WebPages;

namespace Site.Models {
    public static class DbHelper {
        // utilizamos una conexion por cada request para tener problema de bloqueos por datareader
        public static Database CurrentDb() {
            if (HttpContext.Current.Items["CurrentDb"] == null) {
                var retval = new DatabaseWithMVCMiniProfiler("MainConnectionString");
                HttpContext.Current.Items["CurrentDb"] = retval;
                return retval;
            }
            return (Database)HttpContext.Current.Items["CurrentDb"];
        }

        // esta es para usar cuando no queremos reusar la existe
        // para tener algo async o en otra transaccion
        public static Database NewDb() {
            return new DatabaseWithMVCMiniProfiler("MainConnectionString");
        }

        public static void SaveAndLog(this PetaPoco.Database db, object rec) {
            bool IsNew = db.IsNew(rec);
            db.Save(rec);
            db.Log(rec, (IsNew ? "Insert" : "Update"));
        }


        public static void UpsertAndLog(this PetaPoco.Database db, object rec, bool IsNew) {
            db.Upsert(rec, IsNew, true);
        }

        public static void Upsert(this PetaPoco.Database db, object rec, bool IsNew, bool Logear = false) {
            if (IsNew) {
                db.Insert(rec);
            }
            else {
                db.Update(rec);
            }
            if (Logear) db.Log(rec, (IsNew ? "Insert" : "Update"));
        }

        public static void LogDelete(this PetaPoco.Database db, object rec, int id) {
            var pd = Database.PocoData.ForType(rec.GetType());
            var pc = pd.Columns[pd.TableInfo.PrimaryKey];
            string Accion = "Delete";
            int primaryKeyValue = id;
            db.DoLog(pd.TableInfo.TableName, primaryKeyValue, Accion);
        }

        public static void LogDelete(this PetaPoco.Database db, Type t, int id) {
            var pd = Database.PocoData.ForType(t);
            var pc = pd.Columns[pd.TableInfo.PrimaryKey];
            string Accion = "Delete";
            int primaryKeyValue = id;
            db.DoLog(pd.TableInfo.TableName, primaryKeyValue, Accion);
        }



        public static void Log(this PetaPoco.Database db, object rec, string Accion) {
            var pd = Database.PocoData.ForType(rec.GetType());
			var pc = pd.Columns[pd.TableInfo.PrimaryKey];
			int primaryKeyValue = (int)pc.GetValue(rec);
            db.DoLog(pd.TableInfo.TableName, primaryKeyValue, Accion);

        }

        public static void Log(this PetaPoco.Database db, Type t, int id, string Accion) {
            var pd = Database.PocoData.ForType(t);
            var pc = pd.Columns[pd.TableInfo.PrimaryKey];
            int primaryKeyValue = id;
            db.DoLog(pd.TableInfo.TableName, primaryKeyValue, Accion);

        }

        private static void DoLog(this PetaPoco.Database db, string NombreTabla, int id, string Accion) {
            int? UsuarioID = (Sitio.Usuario == null ? (NombreTabla == "Usuario" ? id : 0) : Sitio.Usuario.UsuarioID);
            if (UsuarioID == 0) UsuarioID = null;
            db.Execute("INSERT INTO Log (NombreTabla, ID, Fecha, Tipo, UsuarioID) VALUES (@0, @1, @2, @3, @4)", NombreTabla, id, DateTime.Now, Accion, UsuarioID);
        }

        public static List<Log> ListLog(this PetaPoco.Database db, string NombreTabla, int id) {
            var sql = Models.Log.BaseQuery(50);
            sql.Append("WHERE NombreTabla = @0", NombreTabla);
            sql.Append("AND Log.ID = @0", id);
            sql.Append("ORDER BY Fecha DESC");
            return db.Fetch<Log>(sql);
        }


        public static string ModificadoPor(this PetaPoco.Database db, object rec) {
            var pd = Database.PocoData.ForType(rec.GetType());
            var pc = pd.Columns[pd.TableInfo.PrimaryKey];
            int primaryKeyValue = (int)pc.GetValue(rec);

            return db.SingleOrDefault<string>(@"SELECT TOP 1 Usuario.Nombre
                FROM Usuario 
                INNER JOIN Log ON Usuario.UsuarioID = Log.UsuarioID
                WHERE (Log.Tipo = @0 OR Log.Tipo = @1) 
                AND Log.NombreTabla = @2
                AND Log.ID = @3
                ORDER BY Fecha DESC", "Insert", "Update", pd.TableInfo.TableName, primaryKeyValue) ?? "";

        }

        public static void AppendSelectTop(this PetaPoco.Sql sql, int CantidadPorPagina)
        {
            if (CantidadPorPagina > 0)
            {
                sql.Append("SELECT TOP " + CantidadPorPagina.ToString());
            }
            else {
                sql.Append("SELECT");
            }
        }

        public static void AppendKeywordMatching(this PetaPoco.Sql sql, string q, params string[] campos) {
            DoAppendKeywordMatching(sql, q, "AND", campos);
        }

        public static void AppendKeywordMatchingWithOr(this PetaPoco.Sql sql, string q, params string[] campos) {
            DoAppendKeywordMatching(sql, q, "OR", campos);
        }

        private static void DoAppendKeywordMatching(this PetaPoco.Sql sql, string q, string AndOr, params string[] campos) {
            foreach (string keyword in q.Split(' ')) {
                if (!keyword.IsEmpty()) { 
                    var sqlstmt = new StringBuilder();
                    sqlstmt.Append(AndOr + " (");
                    for (int i = 0; i < campos.Length; i++) {
                        sqlstmt.AppendFormat("{0} like @0", campos[i]);
                        if (i != campos.Length - 1)
                            sqlstmt.AppendFormat(" OR ");
                    }
                    sqlstmt.AppendFormat(") ");
                    sql.Append(sqlstmt.ToString(), "%" + keyword + "%");
                }
            }
        }
        public static string EnCastellanoPorFavor(Exception ex) {
            if (ex.Message.Contains("DELETE statement conflicted with the REFERENCE constraint")) {
                return "No se puede borrar porque el registro es referenciado por otra entidad";
            }
            else {
                return ex.Message;
            }
        }

        public static string ConnString(string DB = "Main") {
            return System.Configuration.ConfigurationManager.ConnectionStrings[DB + "ConnectionString"].ConnectionString;
        }


    }
	public class DatabaseWithMVCMiniProfiler : PetaPoco.Database
	{
		public DatabaseWithMVCMiniProfiler(IDbConnection connection) : base(connection) { }
        public DatabaseWithMVCMiniProfiler(string connectionStringName) : base(connectionStringName) { }
        public DatabaseWithMVCMiniProfiler(string connectionString, string providerName) : base(connectionString, providerName) { }
        public DatabaseWithMVCMiniProfiler(string connectionString, DbProviderFactory dbProviderFactory) : base(connectionString, dbProviderFactory) { }

		public override IDbConnection OnConnectionOpened( IDbConnection connection) {
			// wrap the connection with a profiling connection that tracks timings 
            return new StackExchange.Profiling.Data.ProfiledDbConnection((DbConnection)connection, MiniProfiler.Current);

		}
	}

}