using Acceptance.Domain;
using System.Collections;
using System.Collections.Generic;

namespace Acceptance.ViewModels.Invoice
{
    public class PaymentsViewModel
    {
        public IEnumerable<Payment> Payments { get; set; }
    }
}
