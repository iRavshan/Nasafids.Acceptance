using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Data.Repositories
{
    public interface IApplicantRepository
    {
        Task Create(Applicant applicant);
        void Update(Applicant applicant);
        void Delete(Applicant applicant);
        Task<IEnumerable<Applicant>> GetAll();
        Task<Applicant> GetById(Guid Id);
        Task CompleteAsync();
    }
}
