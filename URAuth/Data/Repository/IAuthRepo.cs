using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using URAuth.Model;

namespace URAuth.Data.Repository
{
    public interface IAuthRepo
    {
        Task<IEnumerable<User>>GetUserAll();
        Task<User>GetUserById(long id);
        bool CheckUserByUserName(string name);
        Task<IEnumerable<Role>> GetRoles();
        void UpdateUser(User u);
        Task<IList<User>> GetApprovalList(long id);
        Task<bool> SaveChanges();
        void DeleteUser(User u);
    }
}