using ShoppingListWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingListWeb.Models
{
    public class EditAccount
    {
        [Required(ErrorMessage = "Podaj imię.")]
        [StringLength(50, ErrorMessage = "Imię musi się składać od 4 do 50 znaków.", MinimumLength = 4)]
        [Display(Name = "Imię")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Podaj nazwisko.")]
        [StringLength(50, ErrorMessage = "Nazwisko musi się składać od 4 do 50 znaków.", MinimumLength = 4)]
        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Podaj adres e-mail.")]
        [StringLength(100, ErrorMessage = "Adres e-mail musi się składać od 4 do 100 znaków.", MinimumLength = 4)]
        [Display(Name = "Adres e-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(500, ErrorMessage = "Hasło musi się składać od 6 do 50 znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Aktualne hasło")]
        public string Password { get; set; }

        [StringLength(500, ErrorMessage = "Hasło musi się składać od 6 do 50 znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("NewPassword", ErrorMessage = "Hasła muszą być identyczne.")]
        public string ConfirmNewPassword { get; set; }

        public string Fullname => $"{Name} {Surname}";
        public string CreatedOnFormatted => CreatedOn.ToString("dd MMM yyyy");
        public string ModifiedOnFormatted => ModifiedOn.HasValue ? ModifiedOn.Value.ToString("dd MMM yyyy") : "---";

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}