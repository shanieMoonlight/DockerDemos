using Microsoft.Extensions.Logging;
using Moq;
using Movies.API;
using Movies.Shared.Models;
using Movies.Shared.Repos;
using Shouldly;

namespace Movies.Tests;

public class MoviesServiceTests
{
    [Fact]
    public async Task GetMoviesAsync_ReturnsMovies_FromRepository()
    {
        // Arrange
        var mockUow = new Mock<IMoviesUnitOfWork>();
        var mockRepo = new Mock<IMoviesRepo>();
        var mockLogger = new Mock<ILogger<MoviesService>>();

        var movies = new List<Movie>
        {
            Movie.Create("A", "Dir A", 2000),
            Movie.Create("B", "Dir B", 2001)
        };

        mockRepo.Setup(r => r.ListAllAsync()).ReturnsAsync(movies);

        var svc = new MoviesService(mockUow.Object, mockRepo.Object, mockLogger.Object);

        // Act
        var result = await svc.GetMoviesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(movies);
        mockRepo.Verify(r => r.ListAllAsync(), Times.Once);
    }

    //--------------------//

    [Fact]
    public async Task GetMovieAsync_ReturnsMovie_WhenFound()
    {
        // Arrange
        var mockUow = new Mock<IMoviesUnitOfWork>();
        var mockRepo = new Mock<IMoviesRepo>();
        var mockLogger = new Mock<ILogger<MoviesService>>();

        var movie = Movie.Create("C", "Dir C", 2002);
        movie.Id = 5;

        mockRepo.Setup(r => r.FirstOrDefaultByIdAsync(5)).ReturnsAsync(movie);

        var svc = new MoviesService(mockUow.Object, mockRepo.Object, mockLogger.Object);

        // Act
        var result = await svc.GetMovieAsync(5);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(movie);
        mockRepo.Verify(r => r.FirstOrDefaultByIdAsync(5), Times.Once);
    }

    //--------------------//

    [Fact]
    public async Task GetRepoTypeAsync_ReturnsRepoType()
    {
        // Arrange
        var mockUow = new Mock<IMoviesUnitOfWork>();
        var mockRepo = new Mock<IMoviesRepo>();
        var mockLogger = new Mock<ILogger<MoviesService>>();

        var svc = new MoviesService(mockUow.Object, mockRepo.Object, mockLogger.Object);

        // Act
        var type = await svc.GetRepoTypeAsync();

        // Assert
        type.ShouldBe(mockRepo.Object.GetType());
    }

    //--------------------//

    [Fact]
    public async Task SeedDataAsync_DoesNotSeed_WhenRepositoryHasData()
    {
        // Arrange
        var mockUow = new Mock<IMoviesUnitOfWork>();
        var mockRepo = new Mock<IMoviesRepo>();
        var mockLogger = new Mock<ILogger<MoviesService>>();

        var existing = new List<Movie> { Movie.Create("X", "DX", 1999) };
        mockRepo.Setup(r => r.ListAllAsync()).ReturnsAsync(existing);

        var svc = new MoviesService(mockUow.Object, mockRepo.Object, mockLogger.Object);

        // Act
        await svc.SeedDataAsync();

        // Assert
        mockRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Movie>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    //--------------------//

    [Fact]
    public async Task SeedDataAsync_AddsAndSaves_WhenRepositoryEmpty()
    {
        // Arrange
        var mockUow = new Mock<IMoviesUnitOfWork>();
        var mockRepo = new Mock<IMoviesRepo>();
        var mockLogger = new Mock<ILogger<MoviesService>>();

        mockRepo.Setup(r => r.ListAllAsync()).ReturnsAsync(new List<Movie>());
        mockRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Movie>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var svc = new MoviesService(mockUow.Object, mockRepo.Object, mockLogger.Object);

        // Act
        await svc.SeedDataAsync();

        // Assert
        mockRepo.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Movie>>(m => m != null && m.Any()), It.IsAny<CancellationToken>()), Times.Once);
        mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}//Cls
