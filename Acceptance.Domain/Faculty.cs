using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceptance.Domain
{
    public class Faculty
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int PriceOfDay { get; set; }
        public int PriceOfNight { get; set; }
        public int PeriodOfDay { get; set; }
        public int PeriodOfNight { get; set; }
    }
}
