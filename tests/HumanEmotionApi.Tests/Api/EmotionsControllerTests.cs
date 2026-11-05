using HumanEmotionApi.Api.Controllers;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Emotions.Commands;
using HumanEmotionApi.Application.Emotions.Queries;
using HumanEmotionApi.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HumanEmotionApi.Tests.Api;

public class EmotionsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly EmotionsController _controller;

    public EmotionsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new EmotionsController(_mockMediator.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetSupportedEmotionTypes_ShouldReturnOkWithList()
    {
        // Arrange
        var types = new List<string> { "Joy", "Sadness", "Anger", "Fear", "Disgust" }.AsReadOnly();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetSupportedEmotionTypesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(types);

        // Act
        var result = await _controller.GetSupportedEmotionTypes(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(types, okResult.Value);
    }

    [Fact]
    public async Task AddEmotion_Valid_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        var request = new AddEmotionRequest(EmotionType.Joy, 80.0);
        var dto = new EmotionDto(Guid.NewGuid(), humanId, "Joy", 80.0, DateTime.UtcNow, null);

        _mockMediator.Setup(m => m.Send(It.Is<AddEmotionCommand>(c => c.HumanId == humanId && c.Type == EmotionType.Joy && c.Percentage == 80.0), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.AddEmotion(humanId, request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(dto, createdResult.Value);
    }

    [Fact]
    public async Task GetHumanEmotions_ShouldReturnOkWithList()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        var list = new List<EmotionDto>
        {
            new(Guid.NewGuid(), humanId, "Joy", 80.0, DateTime.UtcNow, null)
        };

        _mockMediator.Setup(m => m.Send(It.Is<GetHumanEmotionsQuery>(q => q.HumanId == humanId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        // Act
        var result = await _controller.GetHumanEmotions(humanId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(list, okResult.Value);
    }

    [Fact]
    public async Task GetEmotionByType_Found_ShouldReturnOkWithDto()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        var dto = new EmotionDto(Guid.NewGuid(), humanId, "Joy", 80.0, DateTime.UtcNow, null);

        _mockMediator.Setup(m => m.Send(It.Is<GetEmotionByTypeQuery>(q => q.HumanId == humanId && q.Type == EmotionType.Joy), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.GetEmotionByType(humanId, EmotionType.Joy, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task GetEmotionByType_NotFound_ShouldReturnNotFoundProblemDetails()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.Is<GetEmotionByTypeQuery>(q => q.HumanId == humanId && q.Type == EmotionType.Joy), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmotionDto?)null);

        // Act
        var result = await _controller.GetEmotionByType(humanId, EmotionType.Joy, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.IsType<ProblemDetails>(notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateEmotion_Valid_ShouldReturnOkWithUpdatedDto()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        var request = new UpdateEmotionRequest(99.0);
        var dto = new EmotionDto(Guid.NewGuid(), humanId, "Joy", 99.0, DateTime.UtcNow, DateTime.UtcNow);

        _mockMediator.Setup(m => m.Send(It.Is<UpdateEmotionCommand>(c => c.HumanId == humanId && c.Type == EmotionType.Joy && c.Percentage == 99.0), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.UpdateEmotion(humanId, EmotionType.Joy, request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task DeleteEmotion_ShouldReturnNoContent()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.Is<DeleteEmotionCommand>(c => c.HumanId == humanId && c.Type == EmotionType.Joy), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteEmotion(humanId, EmotionType.Joy, CancellationToken.None);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
}

