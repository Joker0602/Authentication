using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using URAuth.Model;

namespace URAuth.Data.Repository
{
    public class AuthRepo : IAuthRepo
    {
        private readonly URDBContext _context;
        public AuthRepo(URDBContext context){
            _context = context;
        }
        public bool CheckUserByUserName(string name)=> _context.Users.Any(x=>x.UserName == name);
        

        public async void DeleteUser(User u)
        {
            _context.Users.Remove(u);
        }

        public Task<IList<User>> GetApprovalList(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Role>> GetRoles()=>await _context.Roles.ToListAsync();

        public async Task<IEnumerable<User>> GetUserAll()=> await _context.Users.ToListAsync();
        

        public async Task<User> GetUserById(long id)=>await  _context.Users.FirstOrDefaultAsync(x=>x.Id==id);
        

        public async Task<bool> SaveChanges()=>await _context.SaveChangesAsync()>0;
        

        public void UpdateUser(User u)
        {
            throw new NotImplementedException();
        }
    }
}