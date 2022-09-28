using System;

namespace Acceptance.ViewModels.Invoice
{
    public class PaymentForApplicant
    {
        public Domain.Applicant Applicant { get; set; }
        public int Sum { get; set; }
    }
}
