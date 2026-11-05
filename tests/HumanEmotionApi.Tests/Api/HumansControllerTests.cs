using HumanEmotionApi.Api.Controllers;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Humans.Commands;
using HumanEmotionApi.Application.Humans.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HumanEmotionApi.Tests.Api;

public class HumansControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly HumansController _controller;

    public HumansControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new HumansController(_mockMediator.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Create_ValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var request = new CreateHumanRequest("Alice");
        var dto = new HumanDto(Guid.NewGuid(), "Alice", "#FFFFFF", 0.0, new List<EmotionDto>(), DateTime.UtcNow, null);

        _mockMediator.Setup(m => m.Send(It.Is<CreateHumanCommand>(c => c.Name == "Alice"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(dto, createdResult.Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithList()
    {
        // Arrange
        var list = new List<HumanDto>
        {
            new(Guid.NewGuid(), "Alice", "#FFFFFF", 0.0, new List<EmotionDto>(), DateTime.UtcNow, null)
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllHumansQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(list, okResult.Value);
    }

    [Fact]
    public async Task GetById_Found_ShouldReturnOkWithHuman()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new HumanDto(id, "Alice", "#FFFFFF", 0.0, new List<EmotionDto>(), DateTime.UtcNow, null);

        _mockMediator.Setup(m => m.Send(It.Is<GetHumanByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.GetById(id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task GetById_NotFound_ShouldReturnNotFoundProblemDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.Is<GetHumanByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HumanDto?)null);

        // Act
        var result = await _controller.GetById(id, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.IsType<ProblemDetails>(notFoundResult.Value);
    }

    [Fact]
    public async Task Update_ValidRequest_ShouldReturnOkWithUpdatedHuman()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateHumanRequest("Bob");
        var dto = new HumanDto(id, "Bob", "#FFFFFF", 0.0, new List<EmotionDto>(), DateTime.UtcNow, DateTime.UtcNow);

        _mockMediator.Setup(m => m.Send(It.Is<UpdateHumanCommand>(c => c.Id == id && c.Name == "Bob"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.Update(id, request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.Is<DeleteHumanCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(id, CancellationToken.None);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
}

