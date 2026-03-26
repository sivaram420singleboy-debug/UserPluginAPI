using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UserPluginAPI.Controllers
{
  [ApiController]
[Route("api/[controller]")]
[Authorize] // 🔥 ADD
public class ImageController : ControllerBase
    {
        [HttpGet("ConvertPNG")]
        public IActionResult ConvertPNG()
        {
            return Ok("Image converted successfully");
        }
    }
}