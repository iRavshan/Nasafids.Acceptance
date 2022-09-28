using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Data.Repositories
{
    public interface IPaymentRepository
    {
        Task Create(Payment payment);
        void Delete(Payment payment);
        Task<IEnumerable<Payment>> GetAll();
        Task CompleteAsync();
    }
}
