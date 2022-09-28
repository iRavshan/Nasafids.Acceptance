using Acceptance.Data.Repositories;
using Acceptance.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Data
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext dbContext;

        public PaymentRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task Create(Payment payment)
        {
            await dbContext.Payments.AddAsync(payment);
        }

        public void Delete(Payment payment)
        {
            dbContext.Remove(payment);
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            return await dbContext.Payments.Include(w => w.Applicant).Include(m => m.Mover).ToListAsync();
        }
    }
}
