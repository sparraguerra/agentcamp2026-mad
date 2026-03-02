using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DndCopilot.Api.Models;
using DndCopilot.Core.Services;

namespace DndCopilot.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DiceController : ControllerBase
{
    private readonly DiceRoller _diceRoller;

    public DiceController(DiceRoller diceRoller)
    {
        _diceRoller = diceRoller;
    }

    [HttpPost("roll")]
    public ActionResult<DiceRollResponse> Roll([FromBody] DiceRollRequest request)
    {
        try
        {
            var result = _diceRoller.Roll(request.Notation);
            
            return Ok(new DiceRollResponse
            {
                Rolls = result.Rolls,
                Modifier = result.Modifier,
                Total = result.Total,
                Description = result.ToString()
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
