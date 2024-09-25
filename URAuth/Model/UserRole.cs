using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace URAuth.Model
{
    public class UserRole:IdentityUserRole<long>
    {
        public long RoleId { get; set; }
        public long UserId { get; set; }
        public virtual User User{get; set;}
        public virtual Role Role{get;set;}
    }
}