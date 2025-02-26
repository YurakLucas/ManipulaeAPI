using Xunit;
using Moq;
using Manipulae.API.Controllers;
using Manipulae.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Manipulae.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manipulae.API.Tests
{
    public class VideosControllerTests
    {
        [Fact]
        public async Task Filter_ReturnsOk_WithListOfVideos()
        {
            // Arrange
            var videosServiceMock = new Mock<IVideosService>();
            var youtubeServiceMock = new Mock<IYouTubeService>();

            videosServiceMock
                .Setup(v => v.FilterAsync(null, null, null, null, null))
                .ReturnsAsync(new List<VideoDto>
                {
                    new VideoDto { Id = 1, Title = "Video Test" }
                });

            var controller = new VideosController(videosServiceMock.Object, youtubeServiceMock.Object);

            // Act
            var result = await controller.Filter(null, null, null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var videos = Assert.IsType<List<VideoDto>>(okResult.Value);
            Assert.Single(videos);
        }
    }
}