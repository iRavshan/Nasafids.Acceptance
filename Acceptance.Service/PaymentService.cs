using Acceptance.Data.Repositories;
using Acceptance.Domain;
using Acceptance.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository paymentRepo;

        public PaymentService(IPaymentRepository paymentRepo)
        {
            this.paymentRepo = paymentRepo;
        }

        public async Task CompleteAsync()
        {
            await paymentRepo.CompleteAsync();
        }

        public async Task Create(Payment payment)
        {
            await paymentRepo.Create(payment);
        }

        public void Delete(Payment payment)
        {
            paymentRepo.Delete(payment);
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            return await paymentRepo.GetAll();
        }
    }
}
