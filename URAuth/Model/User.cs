using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace URAuth.Model
{
    public class User : IdentityUser<long>
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Address { get; set; }

        public bool Status { get; set; } = true;
        [Required]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}