using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Data.Repositories
{
    public interface IFacultyRepository
    {
        Task Create(Faculty faculty);
        void Delete(Faculty faculty);
        void Update(Faculty faculty);
        Task<IEnumerable<Faculty>> GetAll();
        Task<Faculty> GetById(Guid Id);
        Task CompleteAsync();
    }
}
