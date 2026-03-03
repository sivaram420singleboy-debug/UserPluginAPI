using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("ConvertPNG")]
    public IActionResult ConvertPNG()
    {
        return Ok("Image Converted");
    }
}