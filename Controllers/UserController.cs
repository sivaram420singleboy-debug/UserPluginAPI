using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserPluginAPI.Models;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        [HttpGet("getusers")]
        public IActionResult GetUsers()
        {
            return Ok(new List<User>
            {
                new User { Id = 1, Name = "Shiva" },
                new User { Id = 2, Name = "Ram" }
            });
        }
    }
}