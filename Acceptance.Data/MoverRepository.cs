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
    public class MoverRepository : IMoverRepository
    {
        private readonly AppDbContext dbContext;

        public MoverRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task Create(Mover mover)
        {
            await dbContext.Movers.AddAsync(mover);
        }

        public void Delete(Mover mover)
        {
            dbContext.Movers.Remove(mover);
        }

        public async Task<IEnumerable<Mover>> GetAll()
        {
            return await dbContext.Movers
                 .Include(w => w.User).Include(p => p.Faculty).Include(x => x.Payments)
                 .ToListAsync();
        }

        public async Task<Mover> GetById(Guid Id)
        {
            return await dbContext.Movers.FindAsync(Id);
        }

        public void Update(Mover mover)
        {
            var state = dbContext.Movers.Attach(mover);
            state.State = EntityState.Modified;
        }
    }
}
