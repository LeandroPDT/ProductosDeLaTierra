using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DbUp;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.ValidationMessages;
using Site.Models;
using StackExchange.Profiling;

namespace Site {
    public class MvcApplication : System.Web.HttpApplication {

        protected void Application_BeginRequest() {
            if (Request.IsLocal) {
                MiniProfiler.Start();
            } 
        }

        protected void Application_EndRequest() {
            MiniProfiler.Stop();
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(DateTime), new Site.Models.DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new Site.Models.DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(decimal), new Site.Models.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new Site.Models.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(int), new Site.Models.IntegerModelBinder());
            ModelBinders.Binders.Add(typeof(Single?), new Site.Models.SingleModelBinder());
            ModelBinders.Binders.Add(typeof(double), new Site.Models.DoubleModelBinder());
            ModelBinders.Binders.Add(typeof(double?), new Site.Models.DoubleModelBinder());

            ModelMetadataProviders.Current = new Site.Models.CustomModelMetadataProvider();

            // lo suspendo por ahora porque hace dejar de funcionar otras cosas
            // http://blog.gauffin.org/2011/09/easy-model-and-validation-localization-in-asp-net-mvc3/
            var provider = new LocalizedModelValidatorProvider();
            ModelValidatorProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Add(provider);

            var st = new ResourceStringProvider(Resources.LocalizedStrings.ResourceManager);
            ValidationMessageProviders.Clear();
            ValidationMessageProviders.Add(new GriffinStringsProvider(st));
            ValidationMessageProviders.Add(new MvcDataSource());
            ValidationMessageProviders.Add(new DataAnnotationDefaultStrings());


            // tengo que reacomodar los permisos a la nueva era, asi que por un tiempo vamos a tener que chequear este tema 
            //Seguridad.CheckNuevosPermisos();


            // dbup
            // no andaba porque le faltaba permiso de escritura (!?) en App_data
            try {
                var ConnString = System.Configuration.ConfigurationManager.ConnectionStrings["MainConnectionString"].ConnectionString;
                var upgrader = DeployChanges.To
                    .SqlDatabase(ConnString)
                    .WithScriptsFromFileSystem(Server.MapPath("~/App_Data/DbUp"))
                    .LogToTrace()
                    .Build();

                var result = upgrader.PerformUpgrade();
                if (!result.Successful) {
                    if (System.Configuration.ConfigurationManager.AppSettings["IsDeveloper"] != null) {
                        throw result.Error;
                    }
                    else {
                        MailWarning(result.Error);
                    }
                }
            }
            catch (Exception ex) {
                MailWarning(ex);
            }
        }

        public void Application_Error(object sender, EventArgs e) {
            //si estoy debugeando me voy
            if (Request.IsLocal) return;

            Response.Clear();

            // Code that runs when an unhandled error occurs
            bool SendWarning = !Request.IsLocal;
            Exception myException = Server.GetLastError();
            HttpException httpException = myException as HttpException;

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("id", "dummy");
            //solo para que lo agarre la default route
            routeData.Values.Add("codigo", "dummy");
            //solo para que lo agarre la default route

            if (httpException == null) {
                routeData.Values.Add("action", "Index");
            }
            else {
                switch (httpException.GetHttpCode()) {
                    case 404:
                        //Page not found.
                        routeData.Values.Add("action", "HttpError404");
                        SendWarning = false;
                        //no es importante
                        break;
                    case 500:
                        //server error
                        routeData.Values.Add("action", "HttpError500");
                        break;
                    default:
                        routeData.Values.Add("action", "Index");
                        break;
                }
            }



            //mando el mail
            if (SendWarning) MailWarning(myException);

            //Pass exception details to the target error View.
            routeData.Values.Add("Ex", myException);
            routeData.Values.Add("ErrorDescription", myException.Message);

            //Clear the error on server.
            Server.ClearError();

            //Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;


            //Call target Controller and pass the routeData.
            IController errorController = new Site.Controllers.ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));

        }
        public static void MailWarning(Exception Err, string ExtraInfo = "") {
            // dont refer to Request or Response here because it can be called from App_Start
            try {
                Exception elbase = Err.GetBaseException();
                var ErrMessage = new StringBuilder();
                if (elbase != null) {
                    ErrMessage.Append(elbase.ToString());
                }
                else {
                    ErrMessage.Append(Err.ToString());
                }

                ErrMessage.AppendLine();
                ErrMessage.AppendLine("-------------------------");
                if (Sitio.Usuario != null) {
                    ErrMessage.AppendLine("Usuario: " + Sitio.Usuario.UsuarioID.ToString() + " " + Sitio.Usuario.Nombre);
                }


                ErrMessage.AppendLine("-------------------------");
                ErrMessage.AppendLine("QueryString:");
                foreach (string key in HttpContext.Current.Request.QueryString.Keys) {
                    ErrMessage.AppendLine(key + ": " + HttpContext.Current.Request.QueryString[key]);
                }

                ErrMessage.AppendLine("-------------------------");
                ErrMessage.AppendLine("Form:");
                foreach (string key in HttpContext.Current.Request.Form.Keys) {
                    ErrMessage.AppendLine(key + ": " + HttpContext.Current.Request.Form[key]);
                }

                ErrMessage.AppendLine("-------------------------");
                ErrMessage.AppendLine("Extra Info: " + ExtraInfo);


                MailMessage msg = new MailMessage();
                msg.To.Add("leandropalma@live.com.ar");
                msg.Subject = "Error en ProductosDeLaTierra";
                msg.SubjectEncoding = System.Text.Encoding.UTF8;
                msg.Body = "Ocurrio el siguiente error en " + HttpContext.Current.Request.Url.ToString() + Environment.NewLine + ErrMessage;
                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.IsBodyHtml = false;
                msg.Priority = MailPriority.Normal;

                SmtpClient client = new SmtpClient();
                client.Send(msg);

            }
            catch (Exception) {
                //do nothing
            }

        }

    }
}
