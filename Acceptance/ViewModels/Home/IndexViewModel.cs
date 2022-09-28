using Acceptance.Domain;
using System.Collections.Generic;

namespace Acceptance.ViewModels.Home
{
    public class IndexViewModel
    {
        public int ApplicantsCount { get; set; }
        public int ApplicantsCountOwn { get; set; }
        public int ApplicantsCountOnToday { get; set; }
        public int ApplicantsCountOnWeek { get; set; }
        public int ApplicantsCountOnMonts { get; set; }
        public int MoversCount { get; set; }
        public int MoversCountOwn { get; set; }
        public int MoversCountOnToday { get; set; }
        public int MoversCountOnMonth { get; set; }
        public int MoversCountOnWeek { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
