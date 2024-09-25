using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace URAuth.Dtos.UserDto
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool Status { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}