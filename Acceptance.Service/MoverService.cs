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
    public class MoverService : IMoverService
    {
        private readonly IMoverRepository moverRepo;

        public MoverService(IMoverRepository moverRepo)
        {
            this.moverRepo = moverRepo;
        }

        public async Task CompleteAsync()
        {
            await moverRepo.CompleteAsync();
        }

        public async Task Create(Mover mover)
        {
            await moverRepo.Create(mover);
        }

        public void Delete(Mover mover)
        {
            moverRepo.Delete(mover);
        }

        public async Task<IEnumerable<Mover>> GetAll()
        {
            return (await moverRepo.GetAll()).
                OrderByDescending(w => w.RegistrationDateTime);
        }

        public async Task<IEnumerable<Mover>> GetByFullname(string text)
        {
            IEnumerable<Mover> applicants = await moverRepo.GetAll();

            List<Mover> result = new List<Mover>();

            string[] names = text.Split(' ');

            foreach(Mover item in applicants)
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

        public async Task<Mover> GetById(Guid Id)
        {
            return await moverRepo.GetById(Id);
        }

        public async Task<Mover> GetByPassport(string passport)
        {
            IEnumerable<Mover> applicants = await moverRepo.GetAll();

            foreach (var item in applicants)
            {
                if ((item.PassportSeries + item.PassportNumber).Equals(passport))
                {
                    return item;
                }
            }

            return new Mover();
        }

        public async Task<bool> HasMoverByJshir(string jshir)
        {
            Mover exapplicant = (await moverRepo.GetAll()).FirstOrDefault(w => w.JShIR.Equals(jshir));
            if (exapplicant == null) return true;
            return false;
        }

        public async Task<bool> HasMoverByPassport(string passport)
        {
            Mover exapplicant = (await moverRepo.GetAll()).FirstOrDefault(w => (w.PassportSeries + w.PassportNumber).Equals(passport));
            if (exapplicant == null) return true;
            return false;
        }

        public async Task<bool> MoverExistByNumber(string phoneNumber)
        {
            IEnumerable<Mover> movers = await moverRepo.GetAll();

            Mover mover = movers.Where(w => w.PhoneNumber.Equals(phoneNumber))
                                                                .FirstOrDefault();
            if (mover is not null && !mover.Equals(mover))
                return false;
            return true;
        }

        public void Update(Mover mover)
        {
            moverRepo.Update(mover);
        }
    }
}
