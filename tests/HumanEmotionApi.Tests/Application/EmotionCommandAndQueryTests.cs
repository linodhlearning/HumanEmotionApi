using HumanEmotionApi.Application.Emotions.Commands;
using HumanEmotionApi.Application.Emotions.Queries;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;
using Moq;
using Xunit;

namespace HumanEmotionApi.Tests.Application;

public class EmotionCommandAndQueryTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IHumanRepository> _mockHumanRepo;
    private readonly Mock<IEmotionRepository> _mockEmotionRepo;

    public EmotionCommandAndQueryTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockHumanRepo = new Mock<IHumanRepository>();
        _mockEmotionRepo = new Mock<IEmotionRepository>();
        _mockUow.Setup(u => u.Humans).Returns(_mockHumanRepo.Object);
        _mockUow.Setup(u => u.Emotions).Returns(_mockEmotionRepo.Object);
    }

    [Fact]
    public async Task AddEmotionCommandHandler_Valid_ShouldAddEmotionAndSave()
    {
        // Arrange
        var human = new Human("Alice");
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new AddEmotionCommandHandler(_mockUow.Object);
        var command = new AddEmotionCommand(human.Id, EmotionType.Joy, 85.0);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Joy", result.Type);
        Assert.Equal(85.0, result.Percentage);
        Assert.Equal(human.Id, result.HumanId);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddEmotionCommandHandler_HumanNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(humanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Human?)null);

        var handler = new AddEmotionCommandHandler(_mockUow.Object);
        var command = new AddEmotionCommand(humanId, EmotionType.Joy, 50.0);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateEmotionCommandHandler_Valid_ShouldUpdatePercentage()
    {
        // Arrange
        var human = new Human("Alice");
        human.AddEmotion(EmotionType.Fear, 30.0);

        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new UpdateEmotionCommandHandler(_mockUow.Object);
        var command = new UpdateEmotionCommand(human.Id, EmotionType.Fear, 95.0);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(95.0, result.Percentage);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEmotionCommandHandler_HumanNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(humanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Human?)null);

        var handler = new UpdateEmotionCommandHandler(_mockUow.Object);
        var command = new UpdateEmotionCommand(humanId, EmotionType.Fear, 50.0);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteEmotionCommandHandler_ExistingEmotion_ShouldRemoveAndSave()
    {
        // Arrange
        var human = new Human("Alice");
        human.AddEmotion(EmotionType.Anger, 60.0);

        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new DeleteEmotionCommandHandler(_mockUow.Object);
        var command = new DeleteEmotionCommand(human.Id, EmotionType.Anger);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.False(human.HasEmotion(EmotionType.Anger));
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEmotionCommandHandler_HumanNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(humanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Human?)null);

        var handler = new DeleteEmotionCommandHandler(_mockUow.Object);
        var command = new DeleteEmotionCommand(humanId, EmotionType.Anger);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteEmotionCommandHandler_EmotionNotFoundOnHuman_ShouldThrowNotFoundException()
    {
        // Arrange
        var human = new Human("Alice");
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new DeleteEmotionCommandHandler(_mockUow.Object);
        var command = new DeleteEmotionCommand(human.Id, EmotionType.Anger);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task GetHumanEmotionsQueryHandler_Valid_ShouldReturnAllEmotions()
    {
        // Arrange
        var human = new Human("Alice");
        human.AddEmotion(EmotionType.Joy, 40.0);
        human.AddEmotion(EmotionType.Disgust, 20.0);

        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new GetHumanEmotionsQueryHandler(_mockUow.Object);

        // Act
        var result = await handler.Handle(new GetHumanEmotionsQuery(human.Id), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.Type == "Joy" && e.Percentage == 40.0);
        Assert.Contains(result, e => e.Type == "Disgust" && e.Percentage == 20.0);
    }

    [Fact]
    public async Task GetHumanEmotionsQueryHandler_HumanNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(humanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Human?)null);

        var handler = new GetHumanEmotionsQueryHandler(_mockUow.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetHumanEmotionsQuery(humanId), CancellationToken.None));
    }

    [Fact]
    public async Task GetEmotionByTypeQueryHandler_Found_ShouldReturnEmotionDto()
    {
        // Arrange
        var human = new Human("Alice");
        human.AddEmotion(EmotionType.Sadness, 70.0);

        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new GetEmotionByTypeQueryHandler(_mockUow.Object);

        // Act
        var result = await handler.Handle(new GetEmotionByTypeQuery(human.Id, EmotionType.Sadness), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Sadness", result.Type);
        Assert.Equal(70.0, result.Percentage);
    }

    [Fact]
    public async Task GetEmotionByTypeQueryHandler_NotFound_ShouldReturnNull()
    {
        // Arrange
        var human = new Human("Alice");
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new GetEmotionByTypeQueryHandler(_mockUow.Object);

        // Act
        var result = await handler.Handle(new GetEmotionByTypeQuery(human.Id, EmotionType.Sadness), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEmotionByTypeQueryHandler_HumanNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var humanId = Guid.NewGuid();
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(humanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Human?)null);

        var handler = new GetEmotionByTypeQueryHandler(_mockUow.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetEmotionByTypeQuery(humanId, EmotionType.Sadness), CancellationToken.None));
    }

    [Fact]
    public async Task GetSupportedEmotionTypesQueryHandler_ShouldReturnFiveBasicEmotions()
    {
        // Arrange
        var handler = new GetSupportedEmotionTypesQueryHandler();

        // Act
        var result = await handler.Handle(new GetSupportedEmotionTypesQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Contains("Joy", result);
        Assert.Contains("Sadness", result);
        Assert.Contains("Anger", result);
        Assert.Contains("Fear", result);
        Assert.Contains("Disgust", result);
    }
}

