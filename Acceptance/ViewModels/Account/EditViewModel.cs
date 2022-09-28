using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Account
{
    public class EditViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Foydalanuvchi nomi")]
        public string Username { get; set; }

        [Display(Name = "Holati")]
        public string Enabled { get; set; }

        [Display(Name = "Roli")]
        public string Role { get; set; }
    }
}
