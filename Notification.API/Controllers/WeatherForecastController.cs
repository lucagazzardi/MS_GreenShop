using Microsoft.AspNetCore.Mvc;
using Notification.API.Data;
using Notification.API.Model;

namespace Notification.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly NotificationContext _notContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, NotificationContext notContext)
        {
            _logger = logger;
            _notContext = notContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("Not")]
        public async Task<List<NotificationTemplate>> Not()
        {
            return(await _notContext.GetAsync());
        }
    }
}