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
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepo;

        public UserService(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }

        public async Task CompleteAsync()
        {
            await userRepo.CompleteAsync();
        }

        public async Task Create(User user)
        {
            await userRepo.Create(user);
        }

        public void Delete(User user)
        {
            userRepo.Delete(user);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await userRepo.GetAll();
        }

        public async Task<User> GetById(Guid Id)
        {
            return await userRepo.GetById(Id);
        }

        public void Update(User user)
        {
            userRepo.Update(user);
        }
    }
}
