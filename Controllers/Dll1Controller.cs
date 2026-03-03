using Microsoft.AspNetCore.Mvc;

namespace DLL1.Controllers
{
    [ApiController]
    [Route("api/plugin/dll1")]
    public class Dll1Controller : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("DLL1 Working");
        }
    }
}