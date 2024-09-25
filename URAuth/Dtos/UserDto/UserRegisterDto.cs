using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace URAuth.Dtos.UserDto
{
    public class UserRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public bool Status { get; set; }
        public string[] Roles { get; set; }

    }
}