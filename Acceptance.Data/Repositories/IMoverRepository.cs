using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Data.Repositories
{
    public interface IMoverRepository
    {
        Task Create(Mover mover);
        void Update(Mover mover);
        void Delete(Mover mover);
        Task<IEnumerable<Mover>> GetAll();
        Task<Mover> GetById(Guid Id);
        Task CompleteAsync();
    }
}
