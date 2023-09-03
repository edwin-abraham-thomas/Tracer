using Domain.ApiModels;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SampleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _service;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IWeatherForecastService service, ILogger<WeatherForecastController> logger)
    {
        _service=service;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<ActionResult<WeatherForecast>> GetAsync()
    {
        var forecast = await _service.GetAsync(new CancellationToken());

        if(forecast == null)
        {
            return NotFound();
        }

        return Ok(forecast);
    }
}