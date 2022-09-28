using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Applicant
{
    public class AcceptanceViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Ta'lim yo'nalishi")]
        public string Faculty { get; set; }

        [Display(Name = "F.I.Sh")]
        [Required(ErrorMessage = "F.I.Sh ni kiriting")]
        public string FullName { get; set; }

        [Display(Name = "Ta'lim shakli")]
        [Required(ErrorMessage = "Ta'lim shaklini tanlang")]
        public string TypeOfEducation { get; set; }

        [Display(Name = "Telefon raqam")]
        [Required(ErrorMessage = "Telefon raqamni kiriting")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ikkinchi telefon raqam")]
        [Required(ErrorMessage = "Telefon raqamni kiriting")]
        public string SecondPhoneNumber { get; set; }

        [Display(Name = "Passport yoki ID seriyasi")]
        [Required(ErrorMessage = "Passport seriyasini kiriting")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Passport seriyasi xato")]
        [StringLength(2)]
        public string PassportSeries { get; set; }

        [Display(Name = "Passport yoki ID raqami")]
        [Required(ErrorMessage = "Passport raqamini kiriting")]
        [StringLength(7, ErrorMessage = "Passport raqami 7 ta raqamdan iborat bo'lishi kerak")]
        public string PassportNumber { get; set; }

        [Display(Name = "JShIR raqami")]
        [Required(ErrorMessage = "JShIR raqamini kiriting")]
        [StringLength(14, ErrorMessage = "JShIR 14 ta raqamdan iborat bo'lishi kerak")]
        [MaxLength(14, ErrorMessage = "JShIR 14 ta raqamdan iborat bo'lishi kerak")]
        [MinLength(14, ErrorMessage = "JShIR 14 ta raqamdan iborat bo'lishi kerak")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "JShIR xato kiritilgan")]
        public string JShIR { get; set; }


        [Display(Name = "Yashash manzili")]
        [Required(ErrorMessage = "Yashash manzilini kiriting")]
        public string Location { get; set; }

        [Display(Name = "Ta'lim yo'nalishlari")]
        public IEnumerable<Acceptance.Domain.Faculty> Faculties { get; set; }

        [Display(Name = "Imtihonda to'plagan bali")]
        [Required(ErrorMessage = "Imtihonda to'plagan balini kiriting")]
        public double ExamScore { get; set; }

    }
}
