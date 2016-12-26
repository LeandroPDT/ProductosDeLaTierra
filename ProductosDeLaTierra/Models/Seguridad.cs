using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Site.Models {
    public class Seguridad {

	    public enum Feature {
		    Entrar,
		    Editar,
		    Borrar
	    }
	    public enum Permisos {
		    Base_de_datos_Seguridad = 1,
			Producto = 2,
			EventoEnvio = 3,
			EventoRecepcion = 4,
			EventoVenta = 5,
			EventoPago = 6,
			Cargamento = 7,
            EventoDecomisacion = 8,
            ModificarAyuda = 9,
            Liquidacion = 10,
            Modificar_Cualquier_Incidente = 11,
            Remanente = 12,
            ReporteActividad = 13,
            EventoCobro = 14,
            Cobranza = 15
        }

		public static void CheckNuevosPermisos() {
			var db = DbHelper.CurrentDb();
			var LastPermisoID = db.Single<int>("SELECT MAX(PermisoID) FROM Permiso");

			if (LastPermisoID <= 1) {
				Agregar(Seguridad.Permisos.Producto, "Productos", "", true);
			}
		}

        private static void Duplicar(Permisos Original, Permisos AGenerar, string Nombre, string Notas, bool EsABM) {
            var db = DbHelper.CurrentDb();

            using (var scope = db.GetTransaction()) {
                var newp = new Permiso();
                newp.PermisoID = (int)AGenerar;
                newp.Nombre = Nombre;
                newp.Notas = Notas;
                newp.EsABM = EsABM;
                newp.Activo = true;
                db.Insert(newp);

                // ahora le copio los permisos
                var sqlstmt = @"INSERT INTO PermisoConcedido (PermisoID, RolID, UsuarioID, PuedeEntrar, PuedeBorrar, PuedeEditar)
                    SELECT @0, RolID, UsuarioID, PuedeEntrar, PuedeBorrar, PuedeEditar
                        FROM PermisoConcedido
                        WHERE PermisoID = @1";
                db.Execute(sqlstmt, (int)AGenerar, (int)Original);
                    
                scope.Complete();
            }
        }

        private static void Renombrar(Permisos ElPermiso, string Nombre, string Notas) {
            var p = Permiso.SingleOrDefault((int)ElPermiso);
            p.Nombre = Nombre;
            p.Notas = Notas;
            DbHelper.CurrentDb().Save(p);
        }

        private static void Agregar(Permisos AGenerar, string Nombre, string Notas, bool EsABM) {
            var db = DbHelper.CurrentDb();

            using (var scope = db.GetTransaction()) {
                var newp = new Permiso();
                newp.PermisoID = (int)AGenerar;
                newp.Nombre = Nombre;
                newp.Notas = Notas;
                newp.EsABM = EsABM;
                newp.Activo = true;
                db.Insert(newp);

                scope.Complete();
            }
        }





	    public static bool CanAccess(int PermisoID) {
		    return CanAccessToFunction(Sitio.Usuario.UsuarioID, PermisoID, Feature.Entrar);
	    }

	    public static bool CanEdit(int PermisoID) {
            return CanAccessToFunction(Sitio.Usuario.UsuarioID, PermisoID, Feature.Editar);
	    }

	    public static bool CanDelete(int PermisoID) {
            return CanAccessToFunction(Sitio.Usuario.UsuarioID, PermisoID, Feature.Borrar);
	    }

        // las mismas funciones pero permitiendo pasar el enum
        public static bool CanAccess(Permisos Permiso) {
            return CanAccessToFunction(Sitio.Usuario.UsuarioID, (int)Permiso, Feature.Entrar);
        }

        public static bool CanEdit(Permisos Permiso) {
            return CanAccessToFunction(Sitio.Usuario.UsuarioID, (int)Permiso, Feature.Editar);
        }
        public static bool CanDelete(Permisos Permiso) {
            return CanAccessToFunction(Sitio.Usuario.UsuarioID, (int)Permiso, Feature.Borrar);
        }

        private static List<PermisoConcedido> GetPemisosConcedidos(int UsuarioID) {
            var db = DbHelper.CurrentDb();
            var sqlstmt = @"
            SELECT PermisoConcedido.PermisoID as PermisoID,
                CAST(MAX(CAST(PuedeEntrar as INT)) AS BIT) as PuedeEntrar,
	    		CAST(MAX(CAST(PuedeEditar as INT)) AS BIT) as PuedeEditar,
	    		CAST(MAX(CAST(PuedeBorrar as INT)) AS BIT) as PuedeBorrar
	    	FROM PermisoConcedido
	    	WHERE (PermisoConcedido.UsuarioID = @0 OR PermisoConcedido.RolID IN (SELECT RolID from usuarioRol where usuarioRol.UsuarioID = @0 ))
                AND (PuedeEntrar <> 0 OR PuedeEditar <> 0 OR PuedeBorrar <> 0)
            GROUP BY PermisoConcedido.PermisoID";
            return db.Fetch<PermisoConcedido>(sqlstmt, UsuarioID);
        }


        private static List<PermisoConcedido> PermisosDelUsuario(int UsuarioID) {
            string cacheKey = "PermisosDelUsuario" + UsuarioID.ToString();
            var retval = HttpRuntime.Cache[cacheKey] as List<PermisoConcedido>;
            if (retval == null) {
                retval = GetPemisosConcedidos( UsuarioID );
                HttpRuntime.Cache.Insert(cacheKey, retval, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
            }
            return retval;
        }

	    public static bool CanAccessToFunction(int UsuarioID, int PermisoID, Feature Feature) {
        	var permiso = (from rec in PermisosDelUsuario( UsuarioID ) where rec.PermisoID == PermisoID select rec).FirstOrDefault();
			if (permiso != null) {
		        switch (Feature) {
			        case Feature.Editar:
				        return permiso.PuedeEditar;
			        case Feature.Borrar:
				        return permiso.PuedeBorrar;
			        default:
				        return permiso.PuedeEntrar;
        		}
            }
            return false;
	    }


        public static void CleanCache(int UsuarioID) {
            string cacheKey = "PermisosDelUsuario" + UsuarioID.ToString();
            HttpRuntime.Cache.Remove(cacheKey);
        }

        public static void CleanAllCache() {
            var keysToClear = (from System.Collections.DictionaryEntry dict in HttpRuntime.Cache
                               let key = dict.Key.ToString()
                               where key.StartsWith("PermisosDelUsuario")
                               select key).ToList();

            foreach (var key in keysToClear) {
                HttpRuntime.Cache.Remove(key);
            }

        }

    }



}