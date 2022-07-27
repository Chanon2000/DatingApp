using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

// แต่ละ roles สามารถเป็นได้หลาย roles แต่ละ roles ก็สามารถมีได้หลาย users
// many to many
namespace API.Entities
{
    // AppUserRole เป็น join table
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}