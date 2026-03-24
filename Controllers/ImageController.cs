using Microsoft.AspNetCore.Mvc;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        [HttpGet("ConvertPNG")]
        public IActionResult ConvertPNG()
        {
            return Ok("Image converted successfully");
        }
    }
}