using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingListWeb.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Podaj adres e-mail.")]
        [StringLength(100, ErrorMessage = "Adres e-mail musi się składać od 4 do 100 znaków.", MinimumLength = 4)]
        [Display(Name = "Adres e-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Podaj hasło.")]
        [StringLength(500, ErrorMessage = "Hasło musi się składać od 6 do 50 znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }
}