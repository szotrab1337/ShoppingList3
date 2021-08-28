using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingListWeb.Models
{
    public class User : UserLogin
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Podaj imię.")]
        [StringLength(50, ErrorMessage = "Imię musi się składać od 4 do 50 znaków.", MinimumLength = 4)]
        [Display(Name = "Imię")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Podaj nazwisko.")]
        [StringLength(50, ErrorMessage = "Nazwisko musi się składać od 4 do 50 znaków.", MinimumLength = 4)]
        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła muszą być identyczne.")]
        public string PasswordConfirm { get; set; }

        public bool IsAdmin { get; set; }
    }
}