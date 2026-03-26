using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/plugin/dll1")]
    [Authorize] // 🔐 login required
    public class Dll1Controller : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "DLL1 Endpoint Working ✅",
                time = DateTime.Now
            });
        }
    }
}