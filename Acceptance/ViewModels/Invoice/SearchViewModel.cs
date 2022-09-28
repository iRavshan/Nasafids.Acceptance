using System.ComponentModel.DataAnnotations;

namespace Acceptance.ViewModels.Invoice
{
    public class SearchViewModel
    {
        [Required(ErrorMessage = "Shartnoma raqamini kiriting")]
        [StringLength(14, ErrorMessage = "Shartnoma raqami 14 ta raqamdan iborat bo'lishi kerak")]
        public string SearchText { get; set; }
    }
}
