using Xunit;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Repository.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data;

public class ProjectRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;

    public ProjectRepositoryTests()
    {
        // Setup InMemory Database options
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "ProjectDb")
            .Options;
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllProjectsAsync_ShouldReturnListOfProjects_WhenProjectsExist()
    {
        // Arrange
        using (var context = new AppDbContext(_options))
        {
            context.Database.EnsureDeleted(); // Ensures a clean state
        
            // Create related entities if needed
            var user = new User 
            { 
                Name = "Test User", 
                Email = "test@example.com", 
                PasswordHash = "hashed_password"  // Set required properties
            };
        
            var project1 = new Project
            {
                Title = "Project 1",
                Description = "Description for Project 1",
                CreatedBy = user,  // Ensure CreatedBy is set
                Users = new List<User> { user }  // Add users
            };
            var project2 = new Project
            {
                Title = "Project 2",
                Description = "Description for Project 2",
                CreatedBy = user,  // Ensure CreatedBy is set
                Users = new List<User> { user }  // Add users
            };

            context.Users.Add(user); // Add related entities to the context
            context.Projects.AddRange(project1, project2);
        
            await context.SaveChangesAsync(); // Ensure changes are saved
        }

        // Act
        using (var context = new AppDbContext(_options))
        {
            var repository = new ProjectRepository(context);
            var result = await repository.GetAllProjectsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Project>>(result);
            Assert.Equal(2, result.Count()); // Expecting 2 projects
        }
    }


    [Fact]
    public async System.Threading.Tasks.Task   GetAllProjectsAsync_ShouldReturnEmptyList_WhenNoProjectsExist()
    {
        // Arrange
        using (var context = new AppDbContext(_options))
        {
            context.Database.EnsureDeleted(); // Ensures a clean state
            await context.SaveChangesAsync(); // Ensures changes are committed (even for empty cases)
        }

        // Act
        using (var context = new AppDbContext(_options))
        {
            var repository = new ProjectRepository(context);
            var result = await repository.GetAllProjectsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Expecting no projects
        }
    }
}