using A2.Dashboard.Data;
using A2.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A2.Dashboard.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AnswersController : Controller
{
    private readonly WeatherForecastService _weatherForecastService;
    public AnswersController(WeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    [HttpPost]
    public IActionResult Post ([FromBody] AnswerDto answer)
    {
        _weatherForecastService.AddAnswer(answer);
        return Ok();
    }
}