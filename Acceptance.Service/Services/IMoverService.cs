using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acceptance.Service.Services
{
    public interface IMoverService
    {
        Task Create(Mover mover);
        void Update(Mover mover);
        void Delete(Mover mover);
        Task<IEnumerable<Mover>> GetAll();
        Task<Mover> GetById(Guid Id);
        Task<Mover> GetByPassport(string passport);
        Task<bool> MoverExistByNumber(string phoneNumber);
        Task<bool> HasMoverByJshir(string jshir);
        Task<bool> HasMoverByPassport(string passport);
        Task<IEnumerable<Mover>> GetByFullname(string text);
        Task CompleteAsync();
    }
}
