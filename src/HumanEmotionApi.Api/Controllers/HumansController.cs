using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Humans.Commands;
using HumanEmotionApi.Application.Humans.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HumanEmotionApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HumansController : ControllerBase
{
    private readonly IMediator _mediator;

    public HumansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new human.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(HumanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateHumanRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateHumanCommand(request.Name);
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Retrieves all humans with their emotions and ShowColor.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<HumanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllHumansQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific human by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(HumanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetHumanByIdQuery(id);
        var human = await _mediator.Send(query, cancellationToken);
        if (human == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Human Not Found",
                Detail = $"Human with ID '{id}' was not found.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(human);
    }

    /// <summary>
    /// Updates the name of an existing human.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(HumanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateHumanRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateHumanCommand(id, request.Name);
        var updated = await _mediator.Send(command, cancellationToken);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a human and associated emotions.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteHumanCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

