using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace User.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        [HttpPost("login")]
        public IActionResult IdentityLogin()
        {
            return Ok();
        }
    }
}
