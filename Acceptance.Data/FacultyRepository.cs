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
    public class FacultyRepository : IFacultyRepository
    {
        private readonly AppDbContext dbContext;

        public FacultyRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task Create(Faculty faculty)
        {
            await dbContext.Faculties.AddAsync(faculty);
        }

        public void Delete(Faculty faculty)
        {
            dbContext.Faculties.Remove(faculty);
        }

        public async Task<IEnumerable<Faculty>> GetAll()
        {
            return await dbContext.Faculties.ToListAsync();
        }

        public async Task<Faculty> GetById(Guid Id)
        {
            return await dbContext.Faculties.FindAsync(Id);
        }

        public void Update(Faculty faculty)
        {
            var sat = dbContext.Faculties.Attach(faculty);
            sat.State = EntityState.Modified;
        }
    }
}
