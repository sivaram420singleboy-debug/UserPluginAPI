using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    [HttpGet("addoperation")]
    public IActionResult Add(int a, int b)
    {
        return Ok(a + b);
    }
}