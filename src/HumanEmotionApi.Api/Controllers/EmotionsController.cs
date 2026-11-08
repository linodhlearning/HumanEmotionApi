using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Emotions.Commands;
using HumanEmotionApi.Application.Emotions.Queries;
using HumanEmotionApi.Application.Humans.Queries;
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

    /// <summary>
    /// Renders the HTML fragment of the emotion management UI for HTMX.
    /// </summary>
    [HttpGet("api/humans/{humanId:guid}/emotions/fragment")]
    [Produces("text/html")]
    public async Task<ContentResult> GetEmotionsFragment(
        [FromRoute] Guid humanId,
        CancellationToken cancellationToken)
    {
        var html = await RenderEmotionsHtml(humanId, cancellationToken);
        return Content(html, "text/html");
    }

    /// <summary>
    /// HTMX endpoint: adds an emotion and returns the refreshed HTML fragment.
    /// </summary>
    [HttpPost("api/humans/{humanId:guid}/emotions/htmx")]
    [Produces("text/html")]
    public async Task<ContentResult> AddEmotionHtmx(
        [FromRoute] Guid humanId,
        [FromForm] EmotionType emotionType,
        [FromForm] double percentage,
        CancellationToken cancellationToken)
    {
        var command = new AddEmotionCommand(humanId, emotionType, percentage);
        await _mediator.Send(command, cancellationToken);
        var html = await RenderEmotionsHtml(humanId, cancellationToken);
        return Content(html, "text/html");
    }

    /// <summary>
    /// HTMX endpoint: updates an emotion and returns the refreshed HTML fragment.
    /// </summary>
    [HttpPost("api/humans/{humanId:guid}/emotions/{emotionType}/htmx-update")]
    [Produces("text/html")]
    public async Task<ContentResult> UpdateEmotionHtmx(
        [FromRoute] Guid humanId,
        [FromRoute] EmotionType emotionType,
        [FromForm] double percentage,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEmotionCommand(humanId, emotionType, percentage);
        await _mediator.Send(command, cancellationToken);
        var html = await RenderEmotionsHtml(humanId, cancellationToken);
        return Content(html, "text/html");
    }

    /// <summary>
    /// HTMX endpoint: deletes an emotion and returns the refreshed HTML fragment.
    /// </summary>
    [HttpPost("api/humans/{humanId:guid}/emotions/{emotionType}/htmx-delete")]
    [Produces("text/html")]
    public async Task<ContentResult> DeleteEmotionHtmx(
        [FromRoute] Guid humanId,
        [FromRoute] EmotionType emotionType,
        CancellationToken cancellationToken)
    {
        var command = new DeleteEmotionCommand(humanId, emotionType);
        await _mediator.Send(command, cancellationToken);
        var html = await RenderEmotionsHtml(humanId, cancellationToken);
        return Content(html, "text/html");
    }

    private async Task<string> RenderEmotionsHtml(Guid humanId, CancellationToken cancellationToken)
    {
        var human = await _mediator.Send(new GetHumanByIdQuery(humanId), cancellationToken);
        if (human == null)
        {
            return "<div style='color:red;padding:12px;'>Human not found.</div>";
        }

        var emotions = await _mediator.Send(new GetHumanEmotionsQuery(humanId), cancellationToken);

        var emotionNames = new[] { "", "Joy", "Sadness", "Anger", "Fear", "Disgust" };
        var emotionEmojis = new[] { "", "😊", "😢", "😡", "😨", "🤢" };
        var emotionColors = new[] { "", "#FFD700", "#4169E1", "#DC143C", "#8B008B", "#228B22" };

        var rows = new System.Text.StringBuilder();
        for (int t = 1; t <= 5; t++)
        {
            var emotionType = (EmotionType)t;
            var existing = emotions?.FirstOrDefault(e => e.Type.Equals(emotionType.ToString(), StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                rows.Append($$"""
                    <div class="htmx-card" style="background:#fff;border:1px solid #e2e8f0;border-radius:8px;padding:14px;margin-bottom:12px;box-shadow:0 1px 3px rgba(0,0,0,0.05);">
                        <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:8px;">
                            <div style="display:flex;align-items:center;gap:8px;">
                                <span style="font-size:24px;">{{emotionEmojis[t]}}</span>
                                <strong style="font-size:16px;">{{emotionNames[t]}}</strong>
                            </div>
                            <span style="font-size:16px;font-weight:bold;color:{{emotionColors[t]}};">{{existing.Percentage:F0}}%</span>
                        </div>
                        <div style="background:#edf2f7;border-radius:6px;height:10px;overflow:hidden;margin-bottom:12px;">
                            <div style="background:{{emotionColors[t]}};height:100%;width:{{existing.Percentage}}%;"></div>
                        </div>
                        <div style="display:flex;align-items:center;justify-content:space-between;gap:12px;flex-wrap:wrap;">
                            <form hx-post="http://localhost:5050/api/humans/{{humanId}}/emotions/{{t}}/htmx-update"
                                  hx-target="#htmx-emotions-container"
                                  hx-swap="innerHTML"
                                  style="display:flex;align-items:center;gap:8px;flex:1;">
                                <input type="number" name="percentage" value="{{(int)existing.Percentage}}" min="0" max="100" style="padding:6px 10px;border:1px solid #cbd5e1;border-radius:6px;width:80px;" required />
                                <button type="submit" style="background:#3b82f6;color:white;border:none;padding:6px 14px;border-radius:6px;cursor:pointer;font-weight:500;">Update %</button>
                            </form>
                            <form hx-post="http://localhost:5050/api/humans/{{humanId}}/emotions/{{t}}/htmx-delete"
                                  hx-target="#htmx-emotions-container"
                                  hx-swap="innerHTML"
                                  hx-confirm="Remove {{emotionNames[t]}} from {{human.Name}}?">
                                <button type="submit" style="background:#ef4444;color:white;border:none;padding:6px 12px;border-radius:6px;cursor:pointer;font-weight:500;">Remove</button>
                            </form>
                        </div>
                    </div>
                """);
            }
            else
            {
                rows.Append($$"""
                    <div class="htmx-card" style="background:#f8fafc;border:1px dashed #cbd5e1;border-radius:8px;padding:14px;margin-bottom:12px;">
                        <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:8px;">
                            <div style="display:flex;align-items:center;gap:8px;">
                                <span style="font-size:24px;">{{emotionEmojis[t]}}</span>
                                <strong style="color:#64748b;font-size:16px;">{{emotionNames[t]}}</strong>
                            </div>
                            <span style="font-size:13px;color:#94a3b8;">Not added yet</span>
                        </div>
                        <form hx-post="http://localhost:5050/api/humans/{{humanId}}/emotions/htmx"
                              hx-target="#htmx-emotions-container"
                              hx-swap="innerHTML"
                              style="display:flex;align-items:center;gap:8px;">
                            <input type="hidden" name="emotionType" value="{{t}}" />
                            <label style="font-size:13px;color:#64748b;">Initial %:</label>
                            <input type="number" name="percentage" value="50" min="0" max="100" style="padding:6px 10px;border:1px solid #cbd5e1;border-radius:6px;width:80px;" required />
                            <button type="submit" style="background:#10b981;color:white;border:none;padding:6px 14px;border-radius:6px;cursor:pointer;font-weight:500;">+ Add Emotion</button>
                        </form>
                    </div>
                """);
            }
        }

        var headerHtml = $$"""
            <div style="text-align:center;padding:20px;background:#ffffff;border-radius:12px;border:1px solid #e2e8f0;margin-bottom:20px;box-shadow:0 2px 4px rgba(0,0,0,0.04);">
                <div style="width:110px;height:110px;border-radius:50%;background:{{human.ShowColor}};border:4px solid #ffffff;box-shadow:0 4px 10px rgba(0,0,0,0.15);margin:0 auto 12px;display:flex;align-items:center;justify-content:center;transition:background 0.4s ease;">
                    <span style="font-weight:800;font-size:18px;color:#1e293b;text-shadow:0 1px 2px rgba(255,255,255,0.8);">{{human.EmotionScorePercentage:F0}}%</span>
                </div>
                <h2 style="margin:0 0 4px;font-size:22px;color:#0f172a;">{{human.Name}}</h2>
                <div style="display:inline-block;padding:4px 12px;border-radius:20px;background:#f1f5f9;font-family:monospace;font-size:14px;color:#475569;font-weight:600;">
                    ShowColor: {{human.ShowColor}}
                </div>
                <p style="margin:8px 0 0;font-size:13px;color:#64748b;">
                    Total Emotions Added: {{emotions?.Count ?? 0}} / 5
                </p>
            </div>
        """;

        return $"{headerHtml}<div id='htmx-rows'>{rows}</div>";
    }
}

