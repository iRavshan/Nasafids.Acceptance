using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Display(Name = "Maxsus kalit")]
        [Required(ErrorMessage = "Maxsus kalitni kiriting")]
        [RegularExpression("^[{]?[0-9afA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$", ErrorMessage = "Ushbu kalit yaroqsiz")]
        public string Key { get; set; }

        [Display(Name = "Foydalanuvchi nomi")]
        [Required(ErrorMessage = "Foydalanuvchi nomini kiriting")]
        public string Username { get; set; }

        [Display(Name = "Roli")]
        [Required(ErrorMessage = "Rolni kiriting")]
        public string Role { get; set; }
    }
}
