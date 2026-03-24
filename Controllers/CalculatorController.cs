using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // 🔥 Login compulsory
    public class CalculatorController : ControllerBase
    {
        [HttpGet("add")]
        public IActionResult Add(int a, int b)
        {
            return Ok(a + b);
        }
    }
}