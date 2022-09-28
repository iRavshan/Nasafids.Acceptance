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
    public class FacultyService : IFacultyService
    {
        private readonly IFacultyRepository facultyRepo;

        public FacultyService(IFacultyRepository facultyRepo)
        {
            this.facultyRepo = facultyRepo;
        }

        public async Task CompleteAsync()
        {
            await facultyRepo.CompleteAsync();
        }

        public async Task Create(Faculty faculty)
        {
            await facultyRepo.Create(faculty);
        }

        public void Delete(Faculty faculty)
        {
            facultyRepo.Delete(faculty);
        }

        public async Task<IEnumerable<Faculty>> GetAll()
        {
            return await facultyRepo.GetAll();
        }

        public async Task<Faculty> GetById(Guid Id)
        {
            return await facultyRepo.GetById(Id);
        }

        public void Update(Faculty faculty)
        {
            facultyRepo.Update(faculty);
        }
    }
}
