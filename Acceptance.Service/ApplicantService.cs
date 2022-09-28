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
    public class ApplicantService : IApplicantService
    {
        private readonly IApplicantRepository applicantRepo;

        public ApplicantService(IApplicantRepository applicantRepo)
        {
            this.applicantRepo = applicantRepo;
        }

        public async Task<bool> ApplicantExistByNumber(string phoneNumber)
        {
            IEnumerable<Applicant> applicants = await applicantRepo.GetAll();

            Applicant applicant = applicants.Where(w => w.PhoneNumber.Equals(phoneNumber))
                                                                .FirstOrDefault();
            if (applicant is not null && !applicant.Equals(applicant)) 
                return false;
            return true;
        }

        public async Task CompleteAsync()
        {
            await applicantRepo.CompleteAsync();
        }

        public async Task Create(Applicant applicant)
        {
            await applicantRepo.Create(applicant);
        }

        public void Delete(Applicant applicant)
        {
            applicantRepo.Delete(applicant);
        }

        public async Task<IEnumerable<Applicant>> GetAll()
        {
            return (await applicantRepo.GetAll()).
                    OrderByDescending(w => w.RegistrationDateTime);
        }

        public async Task<IEnumerable<Applicant>> GetByFullname(string fullname)
        {
            IEnumerable<Applicant> applicants = await applicantRepo.GetAll();

            List<Applicant> result = new List<Applicant>();

            string[] names = fullname.Split(' ');

            foreach(Applicant item in applicants)
            {
                foreach (string name in names)
                {
                    if (item.FullName.ToLower().Contains(name.ToLower()))
                    {
                        result.Add(item);
                        break;
                    }
                        
                }
            }

            return result;
        }

        public async Task<Applicant> GetById(Guid Id)
        {
            return await applicantRepo.GetById(Id);
        }

        public async Task<Applicant> GetByPassport(string passport)
        {
            IEnumerable<Applicant> applicants = await applicantRepo.GetAll();

            string ppassport = string.Empty;

            foreach (var item in applicants)
            {
                ppassport = item.PassportSeries + item.PassportNumber;

                if (ppassport.Equals(passport))
                {
                    return item;
                }
            }

            return new Applicant();
        }

        public async Task<bool> HasApplicantByJshir(string jshir)
        {
            Applicant exapplicant = (await applicantRepo.GetAll()).FirstOrDefault(w => w.JShIR.Equals(jshir));
            if (exapplicant == null) return true;
            return false;
        }

        public async Task<bool> HasApplicantByPassport(string passport)
        {
            Applicant exapplicant = (await applicantRepo.GetAll()).FirstOrDefault(w => (w.PassportSeries + w.PassportNumber).Equals(passport));
            if (exapplicant == null) return true;
            return false;
        }

        public void Update(Applicant applicant)
        {
            applicantRepo.Update(applicant);
        }
    }
}
