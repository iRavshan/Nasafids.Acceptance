using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Cabinet
{
    public class IndexViewModel
    {
        [Display(Name = "Passport yoki ID seriya va raqam")]
        [Required(ErrorMessage = "Passport seriya va raqamni kiriting")]
        [MinLength(9, ErrorMessage = "Ma'lumot xato, qayta urinib ko'ring")]
        [MaxLength(9, ErrorMessage = "Ma'lumot xato, qayta urinib ko'ring")]
        public string FullPassportSeriesNumber { get; set; }
    }
}
