using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DndCopilot.Api.Models;
using DndCopilot.Core.Interfaces;

namespace DndCopilot.Api.Controllers;

/// <summary>
/// Controller for NPC Agent operations using observe-decide-act pattern.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NpcAgentController : ControllerBase
{
    private readonly INpcAgentService _npcAgentService;

    public NpcAgentController(INpcAgentService npcAgentService)
    {
        _npcAgentService = npcAgentService;
    }

    /// <summary>
    /// Observes the game context and returns NPC perceptions.
    /// </summary>
    /// <param name="request">The observation request.</param>
    /// <returns>Observation result with perceived game elements.</returns>
    [HttpPost("observe")]
    public async Task<ActionResult<NpcAgentObserveResponse>> Observe([FromBody] NpcAgentObserveRequest request)
    {
        if (string.IsNullOrEmpty(request.NpcName))
        {
            return BadRequest(new { message = "NpcName is required" });
        }

        var coreRequest = new NpcObserveRequest
        {
            NpcId = request.NpcId,
            NpcName = request.NpcName,
            CurrentLocation = request.CurrentLocation,
            NearbyCharacters = request.NearbyCharacters,
            NearbyObjects = request.NearbyObjects,
            GameState = request.GameState ?? new Dictionary<string, object>()
        };

        var result = await _npcAgentService.ObserveAsync(coreRequest);

        return Ok(new NpcAgentObserveResponse
        {
            Success = result.Success,
            NpcId = result.NpcId,
            PerceivedThreats = result.PerceivedThreats,
            PerceivedOpportunities = result.PerceivedOpportunities,
            PerceivedNeutral = result.PerceivedNeutral,
            EnvironmentSummary = result.EnvironmentSummary
        });
    }

    /// <summary>
    /// Decides the next action for an NPC based on observations.
    /// </summary>
    /// <param name="request">The decision request.</param>
    /// <returns>Decision result with chosen action and reasoning.</returns>
    [HttpPost("decide")]
    public async Task<ActionResult<NpcAgentDecideResponse>> Decide([FromBody] NpcAgentDecideRequest request)
    {
        if (string.IsNullOrEmpty(request.NpcName))
        {
            return BadRequest(new { message = "NpcName is required" });
        }

        if (request.Observation == null)
        {
            return BadRequest(new { message = "Observation is required for decision making" });
        }

        var coreRequest = new NpcDecideRequest
        {
            NpcId = request.NpcId,
            NpcName = request.NpcName,
            NpcPersonality = request.NpcPersonality,
            NpcGoal = request.NpcGoal,
            Observation = new NpcObserveResult
            {
                Success = request.Observation.Success,
                NpcId = request.Observation.NpcId,
                PerceivedThreats = request.Observation.PerceivedThreats,
                PerceivedOpportunities = request.Observation.PerceivedOpportunities,
                PerceivedNeutral = request.Observation.PerceivedNeutral,
                EnvironmentSummary = request.Observation.EnvironmentSummary
            },
            AvailableActions = request.AvailableActions
        };

        var result = await _npcAgentService.DecideAsync(coreRequest);

        return Ok(new NpcAgentDecideResponse
        {
            Success = result.Success,
            ChosenAction = result.ChosenAction,
            Reasoning = result.Reasoning,
            TargetEntity = result.TargetEntity,
            Confidence = result.Confidence
        });
    }

    /// <summary>
    /// Executes the decided action and returns the result.
    /// </summary>
    /// <param name="request">The action request.</param>
    /// <returns>Action result with outcome and generated events.</returns>
    [HttpPost("act")]
    public async Task<ActionResult<NpcAgentActResponse>> Act([FromBody] NpcAgentActRequest request)
    {
        if (string.IsNullOrEmpty(request.NpcName))
        {
            return BadRequest(new { message = "NpcName is required" });
        }

        if (string.IsNullOrEmpty(request.Action))
        {
            return BadRequest(new { message = "Action is required" });
        }

        var coreRequest = new NpcActRequest
        {
            NpcId = request.NpcId,
            NpcName = request.NpcName,
            Action = request.Action,
            TargetEntity = request.TargetEntity,
            ActionParameters = request.ActionParameters ?? new Dictionary<string, object>(),
            GameState = request.GameState ?? new Dictionary<string, object>()
        };

        var result = await _npcAgentService.ActAsync(coreRequest);

        return Ok(new NpcAgentActResponse
        {
            Success = result.Success,
            ActionPerformed = result.ActionPerformed,
            Outcome = result.Outcome,
            DialogueLine = result.DialogueLine,
            GeneratedEvents = result.GeneratedEvents.Select(e => new NpcAgentGameEvent
            {
                EventType = e.EventType,
                Source = e.Source,
                Target = e.Target,
                Timestamp = e.Timestamp
            }).ToList()
        });
    }
}
