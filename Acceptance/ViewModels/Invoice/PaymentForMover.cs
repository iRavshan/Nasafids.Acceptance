using System;

namespace Acceptance.ViewModels.Invoice
{
    public class PaymentForMover
    {
        public Domain.Mover Mover { get; set; }
        public int Sum { get; set; }
    }
}
