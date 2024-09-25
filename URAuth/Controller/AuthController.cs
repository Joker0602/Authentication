using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using URAuth.Data;
using URAuth.Data.Repository;
using URAuth.Dtos.UserDto;
using URAuth.Model;

namespace URAuth.Controller
{
    [AllowAnonymous]

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IAuthRepo _repo;
        public AuthController(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            SignInManager<User> signInManager,
            IMapper mapper,
            IAuthRepo repo
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpPost("RegisterUser")]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            var userToCreate = _mapper.Map<User>(userRegisterDto);
            if (_repo.CheckUserByUserName(userRegisterDto.UserName))
                return BadRequest("User with username : " + userRegisterDto.UserName + " already exists!.");
            var Password = passwordGenerater();
            var result = await _userManager.CreateAsync(userToCreate, Password);
            var userToReturn = _mapper.Map<UserReadDto>(userToCreate);

            if (result.Succeeded)
            {
                var currentUserCreated = _userManager.FindByNameAsync(userRegisterDto.UserName).Result;
                if(currentUserCreated==null){
                    return BadRequest("User created but could not be found.");
                }
                IEnumerable<string> ienum = (IEnumerable<string>)userRegisterDto.Roles;
                
                var resultRole = await _userManager.AddToRolesAsync(currentUserCreated, ienum);
                
                if (resultRole.Succeeded)
                {
                    return Ok(new { user = userToReturn, password = Password });
                }

                return BadRequest("User created successfully but roles not added.");
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("{id}",Name ="GetUserByID")]
        public async Task<ActionResult<UserReadDto>>GetUserById(int id){

            User user = await _repo.GetUserById(id);
            if(user==null){
                return NotFound("User Not Found");
            }

            var roles= await _userManager.GetRolesAsync(user);
            UserReadDto userRead = _mapper.Map<UserReadDto>(user);
            userRead.Roles=roles;

            return Ok(roles);
        }

        private string passwordGenerater()
        {
            var chars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];

            }
            var finalString = new String(stringChars);
            return finalString;
        }
        
        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection((Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") ? "AppSettings:DevelopmentToken" : "AppSettings:ProductionToken").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTame() ? DateTime.Now.AddHours(8) : DateTime.Now.AddSeconds(-1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto userForLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);
            if (user == null) return Unauthorized("Username or Password is incorrect");
            if (user.Status == false) return Unauthorized("User Inactive!");
            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);
            
            if (result.Succeeded)
            {
                var userMap = _mapper.Map<UserReadDto>(user);
                if (DateTame())
                {
                    
                    return Ok(new { token = GenerateJwtToken(user).Result });
                }
                return Unauthorized("Application license expired please renew license");
            }

            return Unauthorized("Username or Password is incorrect");
        }

        [Authorize("Admin")]
        [HttpDelete("{id}",Name ="DeleteUser")]
        public async Task<ActionResult>DeleteUser(int id){
            var user = await _repo.GetUserById(id);
            _repo.DeleteUser(user);
            await _repo.SaveChanges();
            return Ok(new {message="Deleted"});
        }

        private bool DateTame()
        {
            DateTime endDate = new DateTime(2025, 04, 01);
            DateTime nowDate = DateTime.Today;
            TimeSpan diffResult = endDate.Subtract(nowDate);
            return (diffResult.Days > 0);
        }

        [HttpPut("{loginID}/ResetePassword/{UserId}")]
        public async Task<ActionResult> ResetPassword(long loginID,long UserId){
            User user=await _userManager.FindByIdAsync(UserId.ToString());
            User UserRP= await _repo.GetUserById(UserId);
            if(user==null) return NotFound("user not found");

            var resetToken= await _userManager.GeneratePasswordResetTokenAsync(user);
            string password = passwordGenerater();
            var resetPasswordToken= await _userManager.ResetPasswordAsync(user,resetToken,password);
            if(resetPasswordToken.Succeeded){
                return Ok(new {resetPassword=resetPasswordToken});
            }
            return Unauthorized("Not Reset !");
        }
    }
}
