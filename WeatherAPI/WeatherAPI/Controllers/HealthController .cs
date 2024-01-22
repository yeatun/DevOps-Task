using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HealthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var healthInfo = new
        {
            Status = "Healthy",
            LastChecked = DateTime.UtcNow,
            Database = CheckDatabase(),
            //ExternalService = await CheckExternalService()
            // Add more checks as needed
        };

        return Ok(healthInfo);
    }

    private string CheckDatabase()
    {
        // Add logic to check database connection and health
        // For simplicity, assume the database is always healthy in this example
        return "Healthy";
    }

  
}
