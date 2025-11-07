using Microsoft.AspNetCore.Mvc;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}
