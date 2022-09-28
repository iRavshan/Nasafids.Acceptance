using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Data.Repositories
{
    public interface IUserRepository
    {
        Task Create(User user);
        void Update(User user);
        void Delete(User user);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(Guid Id);
        Task CompleteAsync();
    }
}
