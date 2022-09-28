using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Faculty
{
    public class FacultiesViewModel
    {
        public IEnumerable<Acceptance.Domain.Faculty> Faculties { get; set; }
    }
}
