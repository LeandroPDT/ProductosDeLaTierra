using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Models {

    // clase helper que ayuda a guardar un dato cacheado por request
    public static class CacheInRequestHelper {
        public static void Add<T>(T o, string key) {
            HttpContext.Current.Items.Add(key, o);
        }

        public static void Clear(string key) {
            HttpContext.Current.Items.Remove(key);
        }

        public static bool Exists(string key) {
            return HttpContext.Current.Items[key] != null;
        }

        public static bool Get<T>(string key, out T value) {
            try {
                if (!Exists(key)) {
                    value = default(T);
                    return false;
                }

                value = (T)HttpContext.Current.Items[key];
            }
            catch {
                value = default(T);
                return false;
            }

            return true;
        }
        public static T Get<T>(string key, Func<T> GetValue) {
            try {
                if (!Exists(key)) {
                    T newValue = GetValue();
                    Add(newValue, key);
                    return newValue;
                }
                return (T)HttpContext.Current.Items[key];
            }
            catch {
                return default(T);
            }
        }
        public static void Update<T>(string key, T o ) {
            HttpContext.Current.Items[key] = o;
        }

    }
}
