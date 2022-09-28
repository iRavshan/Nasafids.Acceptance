using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Faculty
{
    public class CreateViewModel
    {
        [Required]
        [Display(Name = "Nomi")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Raqami")]
        public int Number { get; set; }

        [Required]
        [Display(Name = "Kunduzgi uchun narxi")]
        public int PriceOfDay { get; set; }

        [Display(Name = "Sirtqi uchun narxi")]
        public int PriceOfNight { get; set; }

        [Required]
        [Display(Name = "Kunduzgi uchun narxi")]
        public int PeriodOfDay { get; set; }

        [Required]
        [Display(Name = "Sirtqi uchun narxi")]
        public int PeriodOfNight { get; set; }
    }
}
