using Microsoft.AspNetCore.Mvc;
using Backend.Controllers;
using Backend.Database;
using Backend.Messages; 
using Backend.Entities;
using Backend.DTO;
using MassTransit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Controllers
{
    public class PromptsControllerTests
    {
        [Fact]
        public async Task Create_ShouldSaveToDbAndPublishMessage()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestControllerDb")
                .Options;

            using var context = new AppDbContext(options);
            var mockPublish = new Mock<IPublishEndpoint>();

            var controller = new PromptsController(context, mockPublish.Object);
            var request = new CreatePromptRequest { Content = "Test content" };

            // Act
            var result = await controller.Create(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdPrompt = Assert.IsType<Prompt>(okResult.Value);
            
            // saved in database?
            Assert.Equal("Test content", createdPrompt.Content);
            Assert.Equal(PromptStatus.Pending, createdPrompt.Status);
            
            // only 1 stack call
            mockPublish.Verify(p => p.Publish(It.IsAny<PromptCreated>(), default), Times.Once);        }
    }
}