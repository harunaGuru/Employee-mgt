using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myproject.ViewModels
{
    public class UserClaimViewModel
    {
        public UserClaimViewModel()
        {
            claims = new List<UserClaim>();
        }
        public string UserId { get; set; }
        public List<UserClaim> claims { get; set; }

    }
}
