using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Domain
{
    public class Payment
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Sum { get; set; }
        public Mover Mover { get; set; }
        public Applicant Applicant { get; set; }
    }
}
