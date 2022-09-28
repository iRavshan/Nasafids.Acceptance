using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Service.Services
{
    public interface IApplicantService
    {
        Task Create(Applicant applicant);
        void Update(Applicant applicant);
        void Delete(Applicant applicant);
        Task<IEnumerable<Applicant>> GetAll();
        Task<Applicant> GetById(Guid Id);
        Task<bool> ApplicantExistByNumber(string phoneNumber);
        Task<bool> HasApplicantByJshir(string jshir);
        Task<bool> HasApplicantByPassport(string passport);
        Task<Applicant> GetByPassport(string passport);
        Task<IEnumerable<Applicant>> GetByFullname(string fullname);
        Task CompleteAsync();
    }
}
