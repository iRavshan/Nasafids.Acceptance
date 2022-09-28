using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acceptance.Domain;

namespace Acceptance.ViewModels.Applicant
{
    public class ApplicantsViewModel
    {
        public IEnumerable<Acceptance.Domain.Applicant> Applicants { get; set; }
        public string SearchText { get; set; }

    }
}
