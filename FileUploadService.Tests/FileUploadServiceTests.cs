using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FileUploadService.Controllers;
using FileUploadService.Entities;
using FileUploadService.Models;
using FileUploadService.Services;
using System.IO;
using System.Text;

namespace FileUploadService.Tests;

public class FileUploadServiceTests
{
    [Fact]
    public async Task UploadFile_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var fileName = "test.txt";
        var fileContent = "Hello World!";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        fileMock.Setup(m => m.OpenReadStream()).Returns(ms);

        var fileStorageMock = new Mock<IFileStorageService>();
        fileStorageMock.Setup(service => service.UploadAsync(It.IsAny<IFormFile>()))
                       .ReturnsAsync(new UploadResult
                       {
                           IsSuccess = true,
                           File = new FileUpload
                           {
                               FileName = fileName,
                               FilePath = $"/mocked/path/{fileName}",
                               FileSize = ms.Length
                           }
                       });

        var controller = new FileUploadController(fileStorageMock.Object);

        // Act
        var result = await controller.UploadFile(fileMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var uploadResult = Assert.IsType<UploadResult>(okResult.Value);
        Assert.True(uploadResult.IsSuccess);
    }

    [Fact]
    public async Task UploadFile_WhenFailed_ReturnsBadRequest()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var fileName = "test.txt";
        var fileContent = "Hello World!";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        fileMock.Setup(m => m.OpenReadStream()).Returns(ms);

        var fileStorageMock = new Mock<IFileStorageService>();
        fileStorageMock.Setup(service => service.UploadAsync(It.IsAny<IFormFile>()))
                       .ReturnsAsync(new UploadResult
                       {
                           IsSuccess = false
                       });

        var controller = new FileUploadController(fileStorageMock.Object);

        // Act
        var result = await controller.UploadFile(fileMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var uploadResult = Assert.IsType<UploadResult>(badRequestResult.Value);
        Assert.False(uploadResult.IsSuccess);
    }
}