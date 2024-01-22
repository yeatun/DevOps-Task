using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class WeatherAPIController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public WeatherAPIController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet("weather")]
    public async Task<IActionResult> GetHelloInfo()
    {
        var info = new
        {
            Hostname = Environment.MachineName,
            DateTime = DateTime.UtcNow.ToString("yyMMddHHmm"),
            Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        };

        string apiKey = _configuration["OpenWeatherMapApiKey"];
        string dhakaApiUrl = $"https://api.openweathermap.org/data/2.5/weather?q=Dhaka&appid={apiKey}";

        using var client = _httpClientFactory.CreateClient();
        var dhakaResponse = await client.GetFromJsonAsync<WeatherResponse>(dhakaApiUrl);

        if (dhakaResponse is not null)
        {
            var result = new
            {
                Hostname = info.Hostname,
                DateTime = info.DateTime,
                Version = info.Version,
                Weather = new
                {
                    Dhaka = new
                    {
                        Temperature = (dhakaResponse.Main.Temp - 273.15).ToString("F2"), // Convert Kelvin to Celsius
                        TempUnit = "C"
                    }
                }
            };

            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }
}

public class WeatherResponse
{
    public MainInfo Main { get; set; }
}

public class MainInfo
{
    public double Temp { get; set; }
}
