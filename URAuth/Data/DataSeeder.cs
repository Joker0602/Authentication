using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using URAuth.Model;

namespace URAuth.Data
{
    enum RolesList
    {
        Admin,
        User
    }
    public class DataSeeder
    {
        public static void UrSetting(URDBContext context, UserManager<User> userManager, RoleManager<Role> roleManager){
            if(!roleManager.Roles.Any()){
                var roles = new List<Role>{
                    new Role{Name = RolesList.Admin.ToString()},
                    new Role{Name= RolesList.Admin.ToString()},
                };
                foreach(var role in roles){
                    roleManager.CreateAsync(role).Wait();
                }
            }
            if(!userManager.Users.Any()){
                var admin = new User{
                    UserName ="Admin",
                    FullName ="S",
                    Address ="Gadhinglaj",
                    PhoneNumber ="9359819494",
                    Email ="admin@gmail.com",
                    Status = true
                };
                var result1 = userManager.CreateAsync(admin,"password").Result;
                if (result1.Succeeded)
                {
                    var user = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(user, new[] { RolesList.Admin.ToString(), RolesList.User.ToString() }).Wait();
                }
            }
        }
    }
}