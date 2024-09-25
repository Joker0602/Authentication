using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace URAuth.Dtos.UserDto
{
    public class UserChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}