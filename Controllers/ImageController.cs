using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImageController : ControllerBase
    {
        [HttpGet("convertpng")]
        public IActionResult ConvertPNG()
        {
            return Ok("Image converted successfully");
        }
    }
}