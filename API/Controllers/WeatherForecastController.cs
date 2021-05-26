using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
// เป็น controller ที่สร้างมาพร้อมกับ project
namespace API.Controllers
{
    [ApiController] // หมายถึงว่า class นี้เป็น API controller
    [Route("[controller]")] // เป็น root ของ API controller
    // controller เป็น placeholder ซึ่งถูกแทนที่ด้วยส่วนแรกของชื่อcontroller (WeatherForecast)
    public class WeatherForecastController : ControllerBase
    // และ controller ต้องรับมาจาก ControllerBase ด้วย
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" 
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        
        [HttpGet]  // คือ controller endpoint
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
