using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Security;
using System.ComponentModel;

namespace Site.Models {

    public class ChangePasswordModel {
        [Required(ErrorMessage = "Requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage="Requerido")]
        [StringLength(100, ErrorMessage = "La {0} debe ser de al menos {2} caracteres", MinimumLength = Usuario.MinRequiredPasswordLength)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "La contraseña nueva y su confirmación no coinciden")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordModel {
        public Guid PasswordResetToken { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [StringLength(100, ErrorMessage = "La {0} debe ser de al menos {2} caracteres", MinimumLength = Usuario.MinRequiredPasswordLength)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "La contraseña nueva y su confirmación no coinciden")]
        public string ConfirmPassword { get; set; }
    }


    public class PasswordResetData {
        public string PasswordResetLink { get; set; }
        public DateTime PasswordResetExpiration { get; set; }
		public string Username { get; set; }

    }

    public class LogOnModel {
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordar contraseña")]
        public bool RememberMe { get; set; }
    }

    public class RememberPasswordViewModel {
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(250)]
        public String Email { get; set; }
    }


    public class MisDatosViewModel {
        public int UsuarioID { get; set; }

        [Display(Name = "Usuario")]
        public String Login { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Nombre")]
        [StringLength(50)]
        public String NombreCompleto { get; set; }

        [Display(Name = "Email")]
        [StringLength(250)]
        public String Email { get; set; }

        public String Avatar { get; set; }


    }

}
