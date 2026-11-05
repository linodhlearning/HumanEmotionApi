using HumanEmotionApi.Application.Humans.Commands;
using HumanEmotionApi.Application.Humans.Queries;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Exceptions;
using Moq;
using Xunit;

namespace HumanEmotionApi.Tests.Application;

public class HumanCommandAndQueryTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IHumanRepository> _mockHumanRepo;

    public HumanCommandAndQueryTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockHumanRepo = new Mock<IHumanRepository>();
        _mockUow.Setup(u => u.Humans).Returns(_mockHumanRepo.Object);
    }

    [Fact]
    public async Task CreateHumanCommandHandler_ShouldAddHumanAndReturnDto()
    {
        // Arrange
        var handler = new CreateHumanCommandHandler(_mockUow.Object);
        var command = new CreateHumanCommand("Alice");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
        Assert.Equal("#FFFFFF", result.ShowColor);
        _mockHumanRepo.Verify(r => r.AddAsync(It.Is<Human>(h => h.Name == "Alice"), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateHumanCommandHandler_ExistingHuman_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var human = new Human("OldName");
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new UpdateHumanCommandHandler(_mockUow.Object);
        var command = new UpdateHumanCommand(human.Id, "NewName");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("NewName", result.Name);
        _mockHumanRepo.Verify(r => r.Update(human), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateHumanCommandHandler_NonExistingHuman_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Human?)null);

        var handler = new UpdateHumanCommandHandler(_mockUow.Object);
        var command = new UpdateHumanCommand(id, "NewName");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteHumanCommandHandler_ExistingHuman_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var human = new Human("Alice");
        _mockHumanRepo.Setup(r => r.GetByIdAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new DeleteHumanCommandHandler(_mockUow.Object);
        var command = new DeleteHumanCommand(human.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mockHumanRepo.Verify(r => r.Delete(human), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteHumanCommandHandler_NonExistingHuman_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockHumanRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Human?)null);

        var handler = new DeleteHumanCommandHandler(_mockUow.Object);
        var command = new DeleteHumanCommand(id);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllHumansQueryHandler_ShouldReturnListOfDtos()
    {
        // Arrange
        var humans = new List<Human> { new("Alice"), new("Bob") };
        _mockHumanRepo.Setup(r => r.GetAllWithEmotionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(humans);

        var handler = new GetAllHumansQueryHandler(_mockUow.Object);

        // Act
        var result = await handler.Handle(new GetAllHumansQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, h => h.Name == "Alice");
        Assert.Contains(result, h => h.Name == "Bob");
    }

    [Fact]
    public async Task GetHumanByIdQueryHandler_ExistingHuman_ShouldReturnDto()
    {
        // Arrange
        var human = new Human("Alice");
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(human.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(human);

        var handler = new GetHumanByIdQueryHandler(_mockUow.Object);

        // Act
        var result = await handler.Handle(new GetHumanByIdQuery(human.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public async Task GetHumanByIdQueryHandler_NonExistingHuman_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockHumanRepo.Setup(r => r.GetByIdWithEmotionsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Human?)null);

        var handler = new GetHumanByIdQueryHandler(_mockUow.Object);

        // Act
        var result = await handler.Handle(new GetHumanByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}

