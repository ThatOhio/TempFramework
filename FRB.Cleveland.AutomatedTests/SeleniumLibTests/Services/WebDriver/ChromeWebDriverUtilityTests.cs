using System.Runtime.InteropServices;
using FluentAssertions;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;
using FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver;
using Moq;
using OpenQA.Selenium.Chrome;
using Xunit;
using Xunit.Abstractions;

namespace FRB.Cleveland.AutomatedTests.SeleniumLibTests.Services.WebDriver
{
    public class ChromeWebDriverUtilityTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ChromeWebDriverUtilityTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WebDriverNameNotNull()
        {
            var test = new ChromeWebDriverUtility(OSPlatform.Windows);
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
            var test = new ChromeWebDriverUtility(osPlatform);
            test.OsArchiveName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DefaultDriverOptionsHappyPath()
        {
            var test = new ChromeWebDriverUtility(OSPlatform.Windows);
            var options = test.GetDefaultDriverOptions();
            options.Should().NotBeNull();
            options.Should().BeOfType<ChromeOptions>();
        }

        [Fact]
        public void LocalVersionWindowsHappyPath()
        {
            var mockRegistryService = new Mock<IRegistryService>();
            mockRegistryService.Setup(
                    x => x.GetFileVersionFromRegistry(ChromeWebDriverUtility.ChromeRegistryKey))
                .Returns("LOCAL_VERSION");

            var test = new ChromeWebDriverUtility(OSPlatform.Windows,
                mockRegistryService.Object, null, null);

            var result = test.GetLocalBrowserVersion();
            result.Should().Be("LOCAL_VERSION");
        }

        [Fact]
        public void LocalVersionMacHappyPath()
        {
            var mockPListService = new Mock<IPListService>();
            mockPListService.Setup(x => x.GetVersionFromPList(
                ChromeWebDriverUtility.ChromePlistLocation, ChromeWebDriverUtility.ChromePlistVersionKey))
                .Returns("VERSION");

            var test = new ChromeWebDriverUtility(OSPlatform.OSX,
                null, mockPListService.Object, null);

            test.GetLocalBrowserVersion().Should().Be("VERSION");
        }

        [Fact]
        public void LocalVersionLinunxHappyPath()
        {
            var test = new ChromeWebDriverUtility(OSPlatform.Linux);
            test.GetLocalBrowserVersion().Should().Be("");
        }

        [Fact]
        public void GetWebDriverVersionWithLocalVersion()
        {
            var mockRegistryService = new Mock<IRegistryService>();
            mockRegistryService.Setup(
                    x => x.GetFileVersionFromRegistry(ChromeWebDriverUtility.ChromeRegistryKey))
                .Returns("LOCAL_VERSION");
            var mockWebRequestService = new Mock<IWebRequestService>();
            mockWebRequestService.Setup(x => x.UrlExists(It.IsRegex(".LOCAL_VERSION.")))
                .Returns(true);

            var test = new ChromeWebDriverUtility(OSPlatform.Windows,
                mockRegistryService.Object, null, mockWebRequestService.Object);
            var result = test.GetWebDriverVersion();
            result.Should().NotBeNull();
            result.Should().Be("LOCAL_VERSION");

            // Extra step in this test to ensure cache works.
            test.GetWebDriverVersion();
            mockWebRequestService.Verify(x => x.UrlExists(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetWebDriverVersionWithNonMatchingLocalVersion()
        {
            var mockRegistryService = new Mock<IRegistryService>();
            mockRegistryService.Setup(
                    x => x.GetFileVersionFromRegistry(ChromeWebDriverUtility.ChromeRegistryKey))
                .Returns("LOCAL.VERSION");
            var mockWebRequestService = new Mock<IWebRequestService>();
            mockWebRequestService.Setup(x => x.UrlExists(It.IsRegex(".LOCAL.VERSION.")))
                .Returns(false);
            mockWebRequestService.Setup(x => x.GetStringResponse(It.IsAny<string>()))
                .Returns("STRING_RESPONSE");

            var test = new ChromeWebDriverUtility(OSPlatform.Windows,
                mockRegistryService.Object, null, mockWebRequestService.Object);
            var result = test.GetWebDriverVersion();
            result.Should().NotBeNull();
            result.Should().Be("STRING_RESPONSE");
        }

        [Fact]
        public void GetWebDriverVersionWithNexusException()
        {
            var mockWebRequestService = new Mock<IWebRequestService>();
            mockWebRequestService.Setup(x => x.GetStringResponse(It.IsAny<string>()))
                .Returns("VERSION");

            var test = new ChromeWebDriverUtility(OSPlatform.FreeBSD,
                null, null, mockWebRequestService.Object);
            var result = test.GetWebDriverVersion();
            result.Should().NotBeNull();
            result.Should().Be("VERSION");
        }


    }
}