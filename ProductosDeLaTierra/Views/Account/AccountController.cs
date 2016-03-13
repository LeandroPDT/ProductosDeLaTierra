using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Site.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;


namespace Site.Controllers {
    public class AccountController : Controller {

        private IAuthenticationManager AuthenticationManager {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnUrl) {
            if (ModelState.IsValid) {
                try {
                    if (Usuario.ValidateUser(model.UserName, model.Password)) {
                        //var u = Usuario.SingleOrDefault(model.UserName);
                        //FormsAuthentication.SetAuthCookie(model.UserName, false);
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                        //var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                        var identity = new System.Security.Principal.GenericIdentity(model.UserName, DefaultAuthenticationTypes.ApplicationCookie);
                        var claimsidentity = new ClaimsIdentity(identity);
                        AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimsidentity);


                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
                            return Redirect(returnUrl);
                        }
                        else {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch (Exception ex) {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult LogOff() {
            //FormsAuthentication.SignOut();
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        public ActionResult ChangePassword() {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            if (ModelState.IsValid) {
                try {
                    var db = DbHelper.CurrentDb();
                    var u = Sitio.Usuario;
                    if (u.Password != Usuario.GetHash(model.OldPassword)) {
                        throw new ApplicationException("La contraseña anterior no es correcta");
                    }
                    else {
                        u.Password = Usuario.GetHash(model.NewPassword);
                        //u.LastPasswordChangedDate = DateTime.Today;
                        db.Log(u, "Cambio su contraseña");
                        db.Save(u);
                    }

                    TempData["InfoMessage"] = "Se ha cambiado su contraseña con éxito";
                    return Redirect("/");

                }
                catch (Exception ex) {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult RememberPassword() {
            return View( new RememberPasswordViewModel());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RememberPassword(RememberPasswordViewModel Form) {
		    var db = DbHelper.CurrentDb();

            try {
                // la parseo un poco por las dudas que la haya copiado directo de Outlook
                var address = new System.Net.Mail.MailAddress(Form.Email.Replace("\"", ""));
                Form.Email = address.Address;
            }
            catch (Exception) {
                TempData["ErrorMessage"] = "Dirección de email invalida";
                return View(Form);
            }


            var usuarios = db.Fetch<Usuario>("Select * from Usuario where email = @0", Form.Email);
            if (usuarios.Count == 0) {
                TempData["ErrorMessage"] = "No se encontró ningun usuario con ese email";
                return View(Form);
            }
                
		    foreach (Usuario u in usuarios) {
                // en vez de mandar una password mando una url para que haga un reset
			    //DoResetPassword(u.UsuarioID);
                SendResetLink(u);
		    }
		    TempData["InfoMessage"] = "Se ha enviado un enlace para cambiar su contraseña a su casilla de correo";
		    return Redirect("/Account/Login");

        }

		[HttpPost]
		public ActionResult EnviarResetLink(int id) {
			var u = Usuario.SingleOrDefault(id);
			SendResetLink(u);
			return Content("OK");
		}

        private void SendResetLink(Usuario u) {
            var db = DbHelper.CurrentDb();
            var PasswordResetToken = Guid.NewGuid();
            var PasswordResetExpiration = DateTime.Now.AddHours(2);
            db.Execute("Update Usuario SET PasswordResetToken=@0, PasswordResetExpiration = @1 WHERE UsuarioID=@2", PasswordResetToken, PasswordResetExpiration, u.UsuarioID);

            // mando el link
            //dynamic email = new Postal.Email("ResetLink");
            //email.Nombre = u.NombreCompleto;
            //email.email = u.Email;
            //email.Username = u.Login;
            //email.PasswordResetLink = "Account/ResetPassword/" + PasswordResetToken.ToString();
            //email.PasswordResetExpiration = PasswordResetExpiration;
            //email.Send();

            var VM = new PasswordResetData();
            VM.PasswordResetLink = "Account/ResetPassword/" + PasswordResetToken.ToString();
            VM.PasswordResetExpiration = PasswordResetExpiration;
			VM.Username = u.UserName;

            var email = new System.Net.Mail.MailMessage();
            email.To.Add(u.Email);
            email.Subject = String.Format("Cambio de contraseña en {0}", Sitio.WebsiteURL);
            email.Body = this.RenderViewToString("EmailResetLink", VM);
            email.IsBodyHtml = true;
            email.Send();
        }

        //private void DoResetPassword(int UsuarioID) {
        //    var db = DbHelper.CurrentDb();
        //    var Usuario = db.SingleOrDefault<Usuario>("Select * From Usuario where UsuarioID = @0", UsuarioID);

        //    if (Usuario != null) {
        //        string newpassword = Utiles.randomLetters(4) + Utiles.randomNumber(1000, 9999).ToString();

        //        db.Update<Usuario>("SET US_Password=@0 WHERE UsuarioID=@1",SimpleMembershipProvider.GetHash(newpassword), UsuarioID);
        //        Usuario.Password = newpassword;

        //        //dynamic email = new Postal.Email("ResetPassword");
        //        //email.Nombre = Usuario.NombreCompleto;
        //        //email.email = Usuario.Email;
        //        //email.Username = Usuario.Login;
        //        //email.Password = newpassword;
        //        //email.Send();

        //        var email = new System.Net.Mail.MailMessage();
        //        email.To.Add(Usuario.Email);
        //        if (DateTime.Today < new DateTime(2013, 12, 01)) email.Bcc.Add("eduardo@molteni.net.ar"); // para ver como funciona
        //        email.Subject = String.Format("Sus datos de acceso a {0}", Sitio.WebsiteURL);
        //        email.Body = this.RenderViewToString("EmailResetPassword", Usuario);
        //        email.IsBodyHtml = true;
        //        email.Send();

        //    } else {
        //        throw new ApplicationException("Usuario no encontrado");
        //    }
        //}


        public ActionResult ResetPassword(Guid id) {
            var VM = DbHelper.CurrentDb().SingleOrDefault<ResetPasswordModel>("SELECT TOP 1 PasswordResetToken FROM Usuario where PasswordResetToken=@0 and PasswordResetExpiration >= @1", id, DateTime.Now);
            if (VM == null) {
                TempData["ErrorMessage"] = "El codigo para resetear la contraseña es incorrecto o se ha vencido. Vuelva a generar uno.";
                return RedirectToAction("RememberPassword");
            }
            return View(VM);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model) {
            if (ModelState.IsValid) {
                try {
                    var db = DbHelper.CurrentDb();
                    var u = db.SingleOrDefault<Usuario>("SELECT TOP 1 * FROM Usuario where PasswordResetToken=@0 and PasswordResetExpiration >= @1", model.PasswordResetToken, DateTime.Now);
                    if (u == null) {
                        throw new ApplicationException("El codigo para resetear la contraseña es incorrecto o se ha vencido");
                    }
                    else {
                        u.Password = Usuario.GetHash(model.NewPassword);
                        u.IsLockedOut = false;
                        u.PasswordResetExpiration = null;
                        u.FailedPasswordAttemptCount = 0;
                        //u.LastPasswordChangedDate = DateTime.Today;
                        db.Log(u, "Cambio su contraseña (reset)");
                        db.Save(u);
                    }

                    TempData["InfoMessage"] = "Se ha cambiado su contraseña con éxito";
                    return Redirect("/");

                }
                catch (Exception ex) {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize]
        public ActionResult MisDatos() {
            var o = new MisDatosViewModel();
            o.UsuarioID = Sitio.Usuario.UsuarioID;
            o.Login = Sitio.Usuario.UserName;
            o.NombreCompleto = Sitio.Usuario.Nombre;
            o.Email = Sitio.Usuario.Email;
            o.Avatar = Sitio.Usuario.Avatar;
            return View(o);
        }

        [HttpPost]
        [Authorize]
        public ActionResult MisDatos(MisDatosViewModel form) {
            if (ModelState.IsValid) {
                //actualizo solo los campo que permito actualizar
                var db = DbHelper.CurrentDb();

                var OtroUsuarioConElMismoMail = db.SingleOrDefault<Usuario>("WHERE Email = @0 AND UsuarioID <> @1", form.Email, form.UsuarioID);
                if (OtroUsuarioConElMismoMail != null) {
                    ModelState.AddModelError("Email", "Ya existe otro usuario con ese email.");
                }
                else {
                    Usuario rec = Sitio.Usuario;
                    rec.Nombre = form.NombreCompleto;
                    rec.Email = form.Email;
                    db.UpsertAndLog(rec, false);

                    TempData["InfoMessage"] = "Mis datos guardados con exito";
                    return Redirect("/");
                }
            }
            return View(form);
        }



        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
