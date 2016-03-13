using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BizLibMVC;

namespace Site.Models {
    public static class Sitio {

        public const string WebsiteURL = "productosdelatierra.com.ar";
        public static string URI {
            get {
                return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority; 
            }
        }

        static string _Version = "";
        public static String Version() {
            if (_Version == "") {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                _Version = assembly.GetName().Version.ToString();
            }
            return _Version;
        }

        public static bool EsEmpleado {
            get {
                return (Seguridad.CanAccess(Seguridad.Permisos.Base_de_datos_Seguridad));
            }
        }

        public static string GetPref(string Clave, string ValorPorDefault) {
            string retval = null;
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            var CachedValue = System.Web.HttpContext.Current.Cache.Get(Key);
            if (CachedValue == null) {
                retval = GetSecondaryPref(Clave, ValorPorDefault);
            }
            else {
                retval = CachedValue.ToString();
            }
            return retval;
        }

        public static DateTime GetPref(string Clave, DateTime ValorPorDefault) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            var CachedValue = System.Web.HttpContext.Current.Cache.Get(Key);
            if (CachedValue != null) {
                return (DateTime)CachedValue;
            }
            return ValorPorDefault;
        }

        public static int GetPref(string Clave, int ValorPorDefault) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            var CachedValue = System.Web.HttpContext.Current.Cache.Get(Key);
            if (CachedValue != null) {
                return (int)CachedValue;
            }
            return ValorPorDefault;
        }

        public static bool GetPref(string Clave, bool ValorPorDefault) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            var CachedValue = System.Web.HttpContext.Current.Cache.Get(Key);
            if (CachedValue != null) {
                return (bool)CachedValue;
            }
            return ValorPorDefault;
        }

        public static T GetPref<T>(string Clave, T ValorPorDefault) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            var CachedValue = System.Web.HttpContext.Current.Cache.Get(Key);
            if (CachedValue != null) {
                return (T)CachedValue;
            }
            return ValorPorDefault;
        }
        
        public static List<IDNombrePar> GetPref(string Clave, List<IDNombrePar> ValorPorDefault) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            var CachedValue = System.Web.HttpContext.Current.Cache.Get(Key);
            if (CachedValue != null) {
                return (List<IDNombrePar>)CachedValue;
            }
            return ValorPorDefault;
        }

        public static IDNombrePar GetPref(string Clave, IDNombrePar ValorPorDefault) {
            IDNombrePar retval = null;
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            var CachedValue = (IDNombrePar)System.Web.HttpContext.Current.Cache.Get(Key);
            if (CachedValue == null) {
                retval = GetSecondaryPref(Clave, ValorPorDefault);
            }
            else {
                retval = CachedValue;
            }
            return retval;
        }

        public static int GetPref(string Clave, IDNombrePar ValorPorDefault, ref string Nombre ) {
            IDNombrePar retval = null;
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            var CachedValue = (IDNombrePar)System.Web.HttpContext.Current.Cache.Get(Key);
            if (CachedValue == null) {
                retval = GetSecondaryPref(Clave, ValorPorDefault);
            }
            else {
                retval = CachedValue;
            }
            Nombre = retval.Nombre;
            return retval.ID;

        }


        private static string GetSecondaryPref(string Clave, string ValorPorDefault) {
            // las fechas no las guardamos porque puede ser que haya lio
            if (Clave.ToLower().Contains("fecha") || Clave.Contains("-q") || Clave.Contains("-CantidadPorPagina")) return ValorPorDefault;
            string[] DosPartes = Clave.Split('-');
            if (DosPartes.Length > 1) {
                string retval = GetPref(DosPartes[1], ValorPorDefault);
                return retval;
            }
            return ValorPorDefault;
        }

        private static IDNombrePar GetSecondaryPref(string Clave, IDNombrePar ValorPorDefault) {
            string[] DosPartes = Clave.Split('-');
            if (DosPartes.Length > 1) {
                IDNombrePar retval = GetPref(DosPartes[1], ValorPorDefault);
                if (retval != null) {
                    return retval;
                }
            }
            return ValorPorDefault;
        }
        
        public static void SetPref(string Clave, string Valor) {
            if (Valor == null)
                Valor = "";

            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            System.Web.HttpContext.Current.Cache.Remove(Key);
            System.Web.HttpContext.Current.Cache.Insert(Key, Valor, null, DateTime.Now.AddHours(5), TimeSpan.Zero);
            // tambien seteo el padre para tenga
            // en las cosas simples no lo voy a poner porque puede traer problemas
            // SetSecondaryPref(Nombre, Valor);
        }

        public static void SetPref(string Clave, DateTime? Valor) {
            SetPref(Clave, Valor ?? DateTime.Today);
        }

        public static void SetPref(string Clave, DateTime Valor) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            System.Web.HttpContext.Current.Cache.Remove(Key);
            System.Web.HttpContext.Current.Cache.Insert(Key, Valor, null, DateTime.Now.AddHours(5), TimeSpan.Zero);
        }

        public static void SetPref(string Clave, int Valor) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            System.Web.HttpContext.Current.Cache.Remove(Key);
            System.Web.HttpContext.Current.Cache.Insert(Key, Valor, null, DateTime.Now.AddHours(5), TimeSpan.Zero);
        }

        public static void SetPref(string Clave, bool Valor) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            System.Web.HttpContext.Current.Cache.Remove(Key);
            System.Web.HttpContext.Current.Cache.Insert(Key, Valor, null, DateTime.Now.AddHours(5), TimeSpan.Zero);
        }

        public static void SetPref<T>(string Clave, T Valor) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            System.Web.HttpContext.Current.Cache.Remove(Key);
            System.Web.HttpContext.Current.Cache.Insert(Key, Valor, null, DateTime.Now.AddHours(5), TimeSpan.Zero);
        }
        public static void SetPref(string Clave, List<IDNombrePar> Valor) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            System.Web.HttpContext.Current.Cache.Remove(Key);
            System.Web.HttpContext.Current.Cache.Insert(Key, Valor, null, DateTime.Now.AddHours(5), TimeSpan.Zero);
        }

        public static void SetPref(string Clave, int? ID, string Nombre) {
            var Par = new IDNombrePar() { ID = (ID ?? 0), Nombre = Nombre };
            SetPref(Clave, Par);
        }

        public static void SetPref(string Clave, int ID, string Nombre) {
            var Par = new IDNombrePar() { ID = ID, Nombre = Nombre };
            SetPref(Clave, Par);
        }

        public static void SetPref(string Clave, IDNombrePar Par) {
            string Key = Sitio.Usuario.UsuarioID.ToString() + "Pref-" + Clave;
            System.Web.HttpContext.Current.Cache.Remove(Key);
            if (!string.IsNullOrEmpty(Par.Nombre)) {
                System.Web.HttpContext.Current.Cache.Insert(Key, Par, null, DateTime.Now.AddHours(5), TimeSpan.Zero);
                // tambien seteo el padre para tenga
                SetSecondaryPref(Clave, Par);
            }
        }
 
        private static void SetSecondaryPref(string Clave, string Valor) {
            string[] DosPartes = Clave.Split('-');
            if (DosPartes.Length > 1) {
                SetPref(DosPartes[1], Valor);
            }
        }

        private static void SetSecondaryPref(string Clave, IDNombrePar Par) {
            string[] DosPartes = Clave.Split('-');
            if (DosPartes.Length > 1) {
                SetPref(DosPartes[1], Par);
            }
        }

        public static Usuario Usuario {
            get {
                if (HttpContext.Current.Items["Usuario"] == null) {
                    var db = DbHelper.CurrentDb();
                    var retval = Usuario.SingleOrDefault(System.Web.HttpContext.Current.User.Identity.Name);
                    HttpContext.Current.Items["Usuario"] = retval;
                    return retval;
                }
                else {
                    return (Usuario)HttpContext.Current.Items["Usuario"];
                }
            }
        }


        public static bool EsDeveloper() {
            return UsuarioID_Developer().Contains(Sitio.Usuario.UsuarioID);
        }

        public static List<int> UsuarioID_Developer() {
            return new List<int>(){1,3};
        }


        public static bool RegistrarVisitaReturnBookmarked(string Path, string Titulo) {
            var db = DbHelper.CurrentDb();
            var sql = PetaPoco.Sql.Builder;
            Titulo = Titulo.TextOverflow(97);
            sql.Append(";declare @@retval bit;Update PaginaVisita set Cantidad = Cantidad + 1, @@retval = IsBookmarked");
            sql.Append(", Titulo = @0", Titulo);
            sql.Append(", LastVisited = @0", DateTime.Now);
            sql.Append("where UsuarioID = @0 and Path = @1", Sitio.Usuario.UsuarioID, Path);
            sql.Append(";select @@retval");
            var IsBookmarked = db.SingleOrDefault<bool?>(sql);
            if (IsBookmarked == null) {
                var p = new PaginaVisita();
                p.Path = Path;
                p.Titulo = Titulo;
                p.UsuarioID = Sitio.Usuario.UsuarioID;
                p.Cantidad = 1;
                p.IsBookmarked = false;
                p.Orden = 0;
                p.LastVisited = DateTime.Now;
                db.Save(p);
                IsBookmarked = false;
            }
            return IsBookmarked ?? false;
        }

        public class EmailStyles {
            public const string h1 = "margin: 5px 0px 5px 0px; font-family: Arial, sans-serif; font-size: 14px; font-weight: normal; letter-spacing: 2px; color: #1694CE; border-bottom: solid 1px #1694CE;";
            public const string h3 = "margin: 5px 0px 2px 0px; font-family: Arial, sans-serif; font-size: 12px; font-weight: bold; color: #1694CE; ";
            public const string thstyle = "border: 1px solid #ccc; padding: 2px 5px; background: #eaeaea;font-family: Arial, sans-serif; ";
            public const string tdstyle = "border: 1px solid #ccc; padding: 2px 5px; font-family: Arial, sans-serif; ";
            public const string p = "font-family: Arial, sans-serif; font-size: 11px; font-weight: normal; color: #666;";
            public const string body = "font-family: Arial, sans-serif; font-size: 11px; font-weight: normal; color: #666;";
        }


        public static List<BizMenu> getMainMenu() {
            List<BizMenu> retval = new List<BizMenu>();
            BizMenu m = default(BizMenu);

            if (System.Web.HttpContext.Current.Request.IsAuthenticated) {
                m = new BizMenu("Cargamentos", "/Cargamento", "", "");
                m.subMenu = CargamentoSubMenu();
				retval.Add(m);
				m = new BizMenu("Productos", "/Producto", "", "",8);
				//m.subMenu = EventosSubMenu();
				retval.Add(m);
            }
            return retval;
        }


        public static List<BizMenu> CargamentoSubMenu() {
            var submenu = new List<BizMenu>();
            submenu.Add(new BizMenu("Historial", "/Eventos/Evento/Index", ""));
            return submenu;
        }
        
        public static List<BizMenu> getConfigMenu() {
            var retval = new List<BizMenu>();
            retval.Add(new BizMenu("Seguridad", "", "", ""));
            retval.Add(new BizMenu("Usuarios", "/Config/Usuario", "", "", (int)Seguridad.Permisos.Base_de_datos_Seguridad));
            retval.Add(new BizMenu("Roles", "/Config/Roles", "", "", (int)Seguridad.Permisos.Base_de_datos_Seguridad));
            retval.Add(new BizMenu("Permisos", "/Config/Permisos", "", "", (int)Seguridad.Permisos.Base_de_datos_Seguridad));
            
            


            return retval;
        }



    }
}