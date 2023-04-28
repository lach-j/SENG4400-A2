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
    private readonly AnswerService _answerService;
    public AnswersController(AnswerService answerService)
    {
        _answerService = answerService;
    }

    [HttpPost]
    public IActionResult Post ([FromBody] AnswerDto answer)
    {
        _answerService.AddAnswer(answer);
        return Ok();
    }
}