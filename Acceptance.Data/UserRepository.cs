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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task Create(User user)
        {
            await dbContext.Users.AddAsync(user);
        }

        public void Delete(User user)
        {
            dbContext.Users.Remove(user);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await dbContext.Users.Include(w => w.Applicants).Include(w => w.Movers).ToListAsync();
                        
        }

        public async Task<User> GetById(Guid Id)
        {
            return await dbContext.Users.FindAsync(Id);
        }

        public void Update(User user)
        {
            var state = dbContext.Users.Attach(user);
            state.State = EntityState.Modified;
        }
    }
}
