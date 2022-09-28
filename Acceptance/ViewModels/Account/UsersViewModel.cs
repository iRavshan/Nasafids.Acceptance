using Acceptance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.ViewModels.Account
{
    public class UsersViewModel
    {
        public IEnumerable<User> Users { get; set; }
    }
}
