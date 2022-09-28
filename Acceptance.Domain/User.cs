using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Domain
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Role { get; set; }

        public bool Enabled { get; set; }

        public ICollection<Applicant> Applicants { get; set; } 
        public ICollection<Mover> Movers { get; set; }  
    }
}
