using Microsoft.AspNetCore.Mvc;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/plugin/dll1")]
    public class Dll1Controller : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("DLL1 Endpoint Working");
        }
    }
}