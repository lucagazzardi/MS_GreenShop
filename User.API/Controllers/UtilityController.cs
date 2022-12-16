using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.API.Model.Others;
using User.API.Repository;

namespace User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : Controller
    {
        private readonly IUserService _userRepository;

        public UtilityController(IUserService userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpPost("category/preference/{categoryId}")]
        public IActionResult AddNewPreference(int categoryId)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            if(claimsIdentity == null)
            {
                return NotFound();                
            }

            var userId = claimsIdentity.FindFirst("Id").Value;
            if (userId != null)
            {
                _userRepository.AddNewPreference(userId, categoryId);
                return Ok();
            }
            else
                return NotFound();
        }
    }
}
