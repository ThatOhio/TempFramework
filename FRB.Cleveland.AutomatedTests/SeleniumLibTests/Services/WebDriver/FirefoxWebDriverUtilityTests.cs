using System.Runtime.InteropServices;
using FluentAssertions;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;
using FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver;
using Moq;
using OpenQA.Selenium.Firefox;
using Xunit;

namespace FRB.Cleveland.AutomatedTests.SeleniumLibTests.Services.WebDriver
{
    public class FirefoxWebDriverUtilityTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void WebDriverNameNotNull(int option)
        {
            OSPlatform osPlatform = default;
            if (option == 1)
                osPlatform = OSPlatform.Windows;
            if (option == 2)
                osPlatform = OSPlatform.Linux;
            var test = new FirefoxWebDriverUtility(osPlatform);
            test.WebDriverName.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void OsArchiveNameIsNotNull(int option)
        {
            OSPlatform osPlatform = default;
            if (option == 1)
                osPlatform = OSPlatform.Windows;
            if (option == 2)
                osPlatform = OSPlatform.Linux;
            if (option == 3)
                osPlatform = OSPlatform.OSX;
            var test = new FirefoxWebDriverUtility(osPlatform);
            test.OsArchiveName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DefaultDriverOptionsHappyPath()
        {
            var test = new FirefoxWebDriverUtility(OSPlatform.Windows);
            var options = test.GetDefaultDriverOptions();
            options.Should().NotBeNull();
            options.Should().BeOfType<FirefoxOptions>();
        }

        [Fact]
        public void GetWebDriverVersionCacheCheck()
        {
            var mockWebRequestService = new Mock<IWebRequestService>();
            mockWebRequestService.Setup(
                    x => x.GetRequestUri(It.IsAny<string>()))
                .Returns("TEST/VERSION");
            var test = new FirefoxWebDriverUtility(OSPlatform.Windows,
                null, null, mockWebRequestService.Object);

            test.GetWebDriverVersion().Should().Be("VERSION");

            test.GetWebDriverVersion();
            mockWebRequestService.Verify(
                x => x.GetRequestUri(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetDownloadUrlZipOrTar(int option)
        {
            OSPlatform osPlatform = default;
            var expectedExtension = "FAIL";
            if (option == 1)
            {
                osPlatform = OSPlatform.Windows;
                expectedExtension = ".zip";
            }
            if (option == 2)
            {
                osPlatform = OSPlatform.Linux;
                expectedExtension = ".tar.gz";
            }

            var mockWebRequestService = new Mock<IWebRequestService>();
            mockWebRequestService.Setup(
                    x => x.GetRequestUri(It.IsAny<string>()))
                .Returns("TEST/VERSION");
            var test = new FirefoxWebDriverUtility(osPlatform,
                null, null, mockWebRequestService.Object);
            var result = test.GetDownloadUrl();
            result.Should().Contain("VERSION");
            result.Should().Contain("geckodriver");
            result.Should().EndWith(expectedExtension);
        }


    }
}