using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.API.Model.Others;
using User.Model;

namespace User.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private readonly UserManager<Model.User> _userManager;
        private readonly IConfiguration Configuration;

        public IdentityController(UserManager<Model.User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            Configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> IdentityRegister([FromBody] UserRegistration userInfo)
        {
            if (!ModelState.IsValid)
            {
                return View(userInfo);
            }

            Model.User user = new Model.User()
            {
                Email = userInfo.Email,
                FullName = userInfo.FullName,
                UserName = userInfo.Email
            };

            var result = await _userManager.CreateAsync(user, userInfo.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View(userInfo);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> IdentityLogin([FromBody] UserLogin userLogin)
        {            
            var user = await _userManager.FindByEmailAsync(userLogin.Email);
            if (user != null &&
                await _userManager.CheckPasswordAsync(user, userLogin.Password))
            {
                var issuer = Configuration["Jwt:Issuer"];
                var audience = Configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
                string userRole = (await _userManager.GetRolesAsync(user)).First();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", user.Id),
                        new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                        new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Role, userRole)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var stringToken = tokenHandler.WriteToken(token);

                return Ok(stringToken);
            }
            else
            {
                return BadRequest();
            }
        }
    }


}
