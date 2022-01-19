using System.Runtime.InteropServices;
using FluentAssertions;
using FRB.Cleveland.AutomatedTests.Lib.Abstract;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;
using FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver;
using FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver.Abstract;
using Moq;
using OpenQA.Selenium;
using Xunit;

namespace FRB.Cleveland.AutomatedTests.SeleniumLibTests.Services.WebDriver
{
    public class WebDriverServiceTests
    {
        [Fact]
        public void ChromeBrowserTypeReturnsCorrectUtility()
        {
            WebDriverService.GetDriverUtility(BrowserType.Chrome).Should().BeOfType<ChromeWebDriverUtility>();
        }

        [Fact]
        public void ConstructorOnlySupportsMainOperatingSystems()
        {
            Assert.Throws<TestLibException>(() =>
            {
                new WebDriverService(null, null, null,
                    new Mock<IFileService>().Object, OSPlatform.FreeBSD);
            });
        }

        [Fact]
        public void DefaultConstructorTest()
        {
            var x = new WebDriverService(BrowserType.Chrome);
        }

        [Fact]
        public void DefaultDriverOptionsUsesUtility()
        {
            var driverUtilityMock = new Mock<IWebDriverUtility>();
            var fileServiceMock = new Mock<IFileService>();

            var test = new WebDriverService(driverUtilityMock.Object, null, null,
                fileServiceMock.Object, OSPlatform.Windows);

            var x = test.DefaultDriverOptions;

            driverUtilityMock.Verify(x => x.GetDefaultDriverOptions(), Times.Once);
        }

        [Fact]
        public void GetDriverWithAlreadyDownloadedDriver()
        {
            var driverUtilityMock = new Mock<IWebDriverUtility>();
            driverUtilityMock.Setup(x => x.GetWebDriverVersion()).Returns("VERSION");
            driverUtilityMock.Setup(x => x.WebDriverName).Returns("DriverName");
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            var test = new WebDriverService(driverUtilityMock.Object, null, null,
                fileServiceMock.Object, OSPlatform.Windows);

            var result = test.GetWebDriverPath();
            result.Should().NotBeNull();
            result.Should().Contain("VERSION");
            result.Should().Contain("DriverName");
        }

        [Fact]
        public void GetDriverWithDownload()
        {
            var driverUtilityMock = new Mock<IWebDriverUtility>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);
            var webRequestServiceMock = new Mock<IWebRequestService>();

            var test = new WebDriverService(driverUtilityMock.Object, null,
                webRequestServiceMock.Object, fileServiceMock.Object, OSPlatform.Windows);

            test.GetDriver(new Mock<DriverOptions>().Object);

            fileServiceMock.Verify(x =>
                x.MakeFileExecutable(It.IsAny<string>(), It.IsAny<OSPlatform>()), Times.Once);
        }



        /*[Fact]
        public void HappyPath()
        {
            var mockDownloadService = new Mock<IDownloadService>();
            var mockDriverUtility = new Mock<IWebDriverUtility>();
            mockDriverUtility.Setup(x => x.WebDriverName)
                .Returns("Testing.Driver.Name");

            var test = new WebDriverService(mockDriverUtility.Object);
            test.DownloadService = mockDownloadService.Object;
            // On other platforms we call chmod, considering we do not actually
            // download a file, we will override to windows to skip that step. 
            test.CurrentOsPlatform = OSPlatform.Windows;

            var driverPath = test.GetDriverPath();

            driverPath.Should().Be(test.DownloadFolder + "Testing.Driver.Name");
            mockDownloadService.Verify(x => 
                x.DownloadFile(It.IsAny<string>(), 
                    It.IsAny<string>()), 
                Times.Once);
            mockDownloadService.Verify(x => 
                x.ExtractArchive(It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }*/
    }
}