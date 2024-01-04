

using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

namespace ElasticTutorial.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly Serilog.ILogger _logger;

        public WeatherForecastController(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            try
            {
                var rnd = new Random();
                if(rnd.Next(1, 5) < 2)
                {
                    throw new Exception("Error has happened");
                }

                return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray());
            }
            catch (Exception ex)
            {

                _logger.Fatal(ex, ex.Message);
                return new StatusCodeResult(500);
            }
        }


        [HttpPost(Name = "GetSomeObject")]
        public IActionResult GetSome([FromBody] DummyClass model)
        {
            try
            {
                _logger.Information(JsonSerializer.Serialize(model));
                var rnd = new Random();
                if (rnd.Next(1, 5) < 2)
                {
                    throw new Exception("Error has happened in getSome");
                }

                return Ok(new DummyClass()
                {
                    Name = model.Name + "Changed",
                    Age = model.Age++
                });

            }
            catch (Exception ex)
            {

                _logger.Fatal(ex, ex.Message);
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("OtherRoute")]
        public IActionResult Other([FromQuery]string name)
        {
            return Ok($"Your name is{name}");
        }
    }
}