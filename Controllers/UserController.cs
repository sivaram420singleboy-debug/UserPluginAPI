using Microsoft.AspNetCore.Mvc;
using UserPluginAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔥 ADD
    public class UserController : ControllerBase
    {
        [HttpGet("getusers")]
        public IActionResult GetUsers()
        {
            var users = new List<User>
            {
                new User { Id = 1, Name = "Shiva", Email = "shiva@email.com" },
                new User { Id = 2, Name = "Ram", Email = "ram@email.com" }
            };

            return Ok(users);
        }

        [HttpGet("getuser/{id}")]
        public IActionResult GetUserById(int id)
        {
            return Ok(new User
            {
                Id = id,
                Name = "Single User",
                Email = "single@email.com"
            });
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok("User API Working 🔥");
        }
    }
}