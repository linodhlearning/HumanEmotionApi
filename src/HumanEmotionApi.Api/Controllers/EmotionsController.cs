using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Emotions.Commands;
using HumanEmotionApi.Application.Emotions.Queries;
using HumanEmotionApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HumanEmotionApi.Api.Controllers;

[ApiController]
[Produces("application/json")]
public class EmotionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmotionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lists all 5 basic emotions supported by the system.
    /// </summary>
    [HttpGet("api/emotions/types")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSupportedEmotionTypes(CancellationToken cancellationToken)
    {
        var query = new GetSupportedEmotionTypesQuery();
        var types = await _mediator.Send(query, cancellationToken);
        return Ok(types);
    }

    /// <summary>
    /// Adds an emotion with a percentage (0-100) to a human object.
    /// Updates the human's ShowColor property.
    /// </summary>
    [HttpPost("api/humans/{humanId:guid}/emotions")]
    [ProducesResponseType(typeof(EmotionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddEmotion(
        [FromRoute] Guid humanId,
        [FromBody] AddEmotionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddEmotionCommand(humanId, request.Type, request.Percentage);
        var created = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetEmotionByType),
            new { humanId, emotionType = created.Type },
            created
        );
    }

    /// <summary>
    /// Retrieves all emotions currently added to a human.
    /// </summary>
    [HttpGet("api/humans/{humanId:guid}/emotions")]
    [ProducesResponseType(typeof(IReadOnlyList<EmotionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHumanEmotions(
        [FromRoute] Guid humanId,
        CancellationToken cancellationToken)
    {
        var query = new GetHumanEmotionsQuery(humanId);
        var emotions = await _mediator.Send(query, cancellationToken);
        return Ok(emotions);
    }

    /// <summary>
    /// Retrieves a specific emotion by type for a human.
    /// </summary>
    [HttpGet("api/humans/{humanId:guid}/emotions/{emotionType}")]
    [ProducesResponseType(typeof(EmotionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmotionByType(
        [FromRoute] Guid humanId,
        [FromRoute] EmotionType emotionType,
        CancellationToken cancellationToken)
    {
        var query = new GetEmotionByTypeQuery(humanId, emotionType);
        var emotion = await _mediator.Send(query, cancellationToken);
        if (emotion == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Emotion Not Found",
                Detail = $"Emotion '{emotionType}' was not found on human '{humanId}'.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(emotion);
    }

    /// <summary>
    /// Updates the percentage (0-100) of an existing emotion on a human.
    /// Updates the human's ShowColor property.
    /// </summary>
    [HttpPut("api/humans/{humanId:guid}/emotions/{emotionType}")]
    [ProducesResponseType(typeof(EmotionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEmotion(
        [FromRoute] Guid humanId,
        [FromRoute] EmotionType emotionType,
        [FromBody] UpdateEmotionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEmotionCommand(humanId, emotionType, request.Percentage);
        var updated = await _mediator.Send(command, cancellationToken);
        return Ok(updated);
    }

    /// <summary>
    /// Removes an emotion from a human.
    /// Recalculates the human's ShowColor property.
    /// </summary>
    [HttpDelete("api/humans/{humanId:guid}/emotions/{emotionType}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEmotion(
        [FromRoute] Guid humanId,
        [FromRoute] EmotionType emotionType,
        CancellationToken cancellationToken)
    {
        var command = new DeleteEmotionCommand(humanId, emotionType);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

