using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalMVC.Controllers
{
    public class RegisterModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "You should fill this field in!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You should fill this field in!")]
        [StringLength(30, ErrorMessage = "The field {0} can containt maximum {2} characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You should fill this field in!")]
        [StringLength(30, ErrorMessage = "The field {0} can containt maximum {2} characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You should fill this field in!")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password is too short!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You should fill this field in!")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}