using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Account
{
    public class LoginViewModel
    {
        [Display(Name = "Kirish uchun kalit")]
        [Required(ErrorMessage = "Maxsus kalitni kiriting")]
        [RegularExpression("^[{]?[0-9afA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$", ErrorMessage = "Ushbu kalit yaroqsiz")]
        public string Key { get; set; }
    }
}
