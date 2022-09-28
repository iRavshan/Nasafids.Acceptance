using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Domain
{
    public class Mover
    {
        public Guid Id { get; set; }

        public Faculty Faculty { get; set; }

        public string FullName { get; set; }

        public string TypeOfEducation { get; set; }

        public string PhoneNumber { get; set; }

        public string SecondPhoneNumber { get; set; }

        public string PassportSeries { get; set; }

        public string PassportNumber { get; set; }

        public string JShIR { get; set; }

        public string Location { get; set; }

        public int State { get; set; }

        public DateTime RegistrationDateTime { get; set; }

        public User User { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
