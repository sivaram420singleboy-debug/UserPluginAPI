using Microsoft.AspNetCore.Mvc;

namespace UserPluginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculatorController : ControllerBase
    {

        // Example:
        // /api/Calculator?operation=add&a=5&b=5
        // /api/Calculator?operation=sub&a=10&b=3

        [HttpGet]
        public IActionResult Calculate(string operation, int a = 0, int b = 0)
        {
            double result;

            switch (operation.ToLower())
            {
                case "add":
                    result = a + b;
                    break;

                case "sub":
                case "subtract":
                    result = a - b;
                    break;

                case "mul":
                case "multiply":
                    result = a * b;
                    break;

                case "div":
                case "divide":
                    if (b == 0)
                        return BadRequest("Division by zero not allowed");
                    result = (double)a / b;
                    break;

                default:
                    return BadRequest("Invalid operation. Use add, sub, mul, div");
            }

            return Ok(new
            {
                Operation = operation,
                A = a,
                B = b,
                Result = result
            });
        }
    }
}