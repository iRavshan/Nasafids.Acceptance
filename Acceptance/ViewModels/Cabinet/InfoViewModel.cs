using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Cabinet
{
    public class InfoViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Payment> Payments { get; set; }
    }
}
