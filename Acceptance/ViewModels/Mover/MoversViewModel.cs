using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acceptance.Domain;

namespace Acceptance.ViewModels.Mover
{
    public class MoversViewModel
    {
        public IEnumerable<Acceptance.Domain.Mover> Movers { get; set; }

        public string SearchText { get; set; }
    }
}
