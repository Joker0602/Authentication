using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace URAuth.Model
{
    public class Role:IdentityRole<long>
    {
        public virtual ICollection<UserRole> UserRoles {get; set;}
    }
}