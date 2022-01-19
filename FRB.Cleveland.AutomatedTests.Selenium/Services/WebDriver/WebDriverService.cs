using System.IO;
using System.Runtime.InteropServices;
using FRB.Cleveland.AutomatedTests.Lib.Abstract;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;
using FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver.Abstract;
using OpenQA.Selenium;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver
{
    public class WebDriverService : IWebDriverService
    {
        private readonly IWebDriverUtility _driverUtility;
        private readonly IShellService _shellService;
        private readonly IWebRequestService _webRequestService;
        private readonly IFileService _fileService;
        private OSPlatform _osPlatform;

        /// <summary>
        /// Gets an empty (default) instance of the appropriate DriverOptions for the provided <see cref="BrowserType"/>. 
        /// </summary>
        public DriverOptions DefaultDriverOptions => _driverUtility.GetDefaultDriverOptions();

        /// <summary>
        /// This folder is used as a working folder to download new WebDrivers, and as a home for extracted WebDrivers.
        /// </summary>
        public string DownloadFolder { get; }

        /// <summary>
        /// The WebDriverService automates the process of finding and even downloading, when necessary, selenium WebDrivers.
        /// </summary>
        /// <param name="browserType"></param>
        public WebDriverService(BrowserType browserType) : this(GetDriverUtility(browserType), new ShellService(),
            new WebRequestService(), new FileService(), Helpers.GetCurrentOsPlatform())
        {
        }

        /// <exception cref="TestLibException">Current OS is not supported.</exception>
        public WebDriverService(IWebDriverUtility webDriverUtility, IShellService shellService,
            IWebRequestService webRequestService, IFileService fileService, OSPlatform osPlatform)
        {
            _driverUtility = webDriverUtility;
            _shellService = shellService;
            _webRequestService = webRequestService;
            _fileService = fileService;
            _osPlatform = osPlatform;

            DownloadFolder = Path.GetTempPath() + "Nexus/";
            fileService.CreateDirectory(DownloadFolder);

            // Verify that we are running on Windows, Linux, or OSX. 
            if (osPlatform != OSPlatform.Windows && osPlatform != OSPlatform.Linux && osPlatform != OSPlatform.OSX)
                throw new TestLibException("Current OS is not supported: " + RuntimeInformation.OSDescription);
        }

        /// <summary>
        /// Creates an instance of <see cref="IWebDriverUtility"/> that matches the provided <see cref="BrowserType"/>. 
        /// </summary>
        /// <exception cref="TestLibException">Provided browser type is not supported.</exception>
        public static IWebDriverUtility GetDriverUtility(BrowserType browserType)
        {
            return browserType switch
            {
                BrowserType.Chrome => new ChromeWebDriverUtility(Helpers.GetCurrentOsPlatform()),
                BrowserType.Firefox => new FirefoxWebDriverUtility(Helpers.GetCurrentOsPlatform()),
                // etc
                _ => throw new TestLibException("BrowserType not yet supported: " + browserType)
            };
        }

        /// <summary>
        /// Creates and returns an instance of the appropriate <see cref="IWebDriver"/> that matches the provided <see cref="BrowserType"/>. Downloads the appropriate driver file if that hasn't happened already.
        /// </summary>
        /// <param name="options">The <see cref="DriverOptions"/> to be provided to the new <see cref="IWebDriver"/>, warning, this much be of an appropraite type. The best way to insure this is the case, is to get your option via <see cref="DefaultDriverOptions"/>.</param>
        public IWebDriver GetDriver(DriverOptions options)
        {
            var driverPath = GetWebDriverPath();
            return _driverUtility.CreateWebDriver(_fileService.GetFileParentDirectoryName(driverPath), options);
        }

        /// <summary>
        /// Downloads the appropriate WebDriver, unless a local webdriver is located. 
        /// </summary>
        public string GetWebDriverPath()
        {
            // See if we've already downloaded an archive for this version. 
            var driverVersion = _driverUtility.GetWebDriverVersion();
            var versionFolder = DownloadFolder + $"/{driverVersion}/";
            var finalDriverLocation = versionFolder + _driverUtility.WebDriverName;
            if (_fileService.FileExists(finalDriverLocation))
                return finalDriverLocation;

            // If the file hasn't been downloaded yet, download, make executable, and return.
            _fileService.CreateDirectory(versionFolder);
            var archivePath = versionFolder + _driverUtility.OsArchiveName;
            _webRequestService.DownloadFile(_driverUtility.GetDownloadUrl(), archivePath);
            _fileService.ExtractArchive(archivePath, versionFolder);
            _fileService.MakeFileExecutable(finalDriverLocation, _osPlatform);
            return finalDriverLocation;
        }
    }
}