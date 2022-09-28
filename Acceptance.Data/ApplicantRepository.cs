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
    public class ApplicantRepository : IApplicantRepository
    {
        private readonly AppDbContext dbContext;

        public ApplicantRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task Create(Applicant applicant)
        {
            await dbContext.Applicants.AddAsync(applicant);
        }

        public void Delete(Applicant applicant)
        {
            dbContext.Applicants.Remove(applicant);
        }

        public async Task<IEnumerable<Applicant>> GetAll()
        {
            return await dbContext.Applicants
                .Include(w => w.User).Include(p => p.Faculty).Include(x => x.Payments).
                            ToListAsync();
        }

        public async Task<Applicant> GetById(Guid Id)
        {
            return await dbContext.Applicants.FindAsync(Id);
        }

        public void Update(Applicant applicant)
        {
            var state = dbContext.Applicants.Attach(applicant);
            state.State = EntityState.Modified;
        }
    }
}
