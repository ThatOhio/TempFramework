using System.Runtime.InteropServices;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;
using FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver.Abstract;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver
{
    public class FirefoxWebDriverUtility : BaseWebDriverUtility
    {
        private readonly OSPlatform _osPlatform;
        private const string VersionPlaceholder = "--VERSION--";
        private const string FirefoxDriverBaseUrl = "https://github.com/mozilla/geckodriver/releases/latest";
        private const string FirefoxDriverDownloadUrl =
            "https://github.com/mozilla/geckodriver/releases/download/LATEST/";

        private string _cachedDriverVersion;

        public FirefoxWebDriverUtility(OSPlatform osPlatform)
        : this(osPlatform, new RegistryService(), new PListService(), new WebRequestService())
        {

        }

        public FirefoxWebDriverUtility(OSPlatform osPlatform, IRegistryService registryService,
            IPListService pListService, IWebRequestService webRequestService)
            : base(registryService, pListService, webRequestService)
        {
            _osPlatform = osPlatform;
        }

        /// <inheritdoc />
        public override string WebDriverName
        {
            get
            {
                string driverName;
                if (_osPlatform == OSPlatform.Windows)
                    driverName = "geckodriver.exe";
                else
                    driverName = "geckodriver";
                return driverName;
            }
        }

        /// <inheritdoc />
        public override string OsArchiveName
        {
            get
            {
                string zipName;
                if (_osPlatform == OSPlatform.Windows)
                    zipName = $"geckodriver-{VersionPlaceholder}-win64.zip";
                else if (_osPlatform == OSPlatform.OSX)
                    zipName = $"geckodriver-{VersionPlaceholder}-macos.tar.gz";
                else
                    zipName = $"geckodriver-{VersionPlaceholder}-linux64.tar.gz";
                return zipName;
            }
        }

        /// <inheritdoc />
        public override string GetDownloadUrl()
        {
            // Get the current version, and construct the archive name with it. 
            var version = GetWebDriverVersion();
            var archiveName = OsArchiveName.Replace(VersionPlaceholder, version);

            // Construct the Url and return.
            return FirefoxDriverDownloadUrl.Replace("LATEST", version) + archiveName;
        }

        /// <inheritdoc />
        public override string GetWebDriverVersion()
        {
            if (!string.IsNullOrWhiteSpace(_cachedDriverVersion))
                return _cachedDriverVersion;

            // Get the latest version and construct the archive name. 
            // When hitting the /latest url of github, the response Uri will be the latest tag
            // We can then use the latest tag to get the version we want, using it to construct the archive name.
            var latestUrl = WebRequestService.GetRequestUri(FirefoxDriverBaseUrl);
            var version = latestUrl.Substring(latestUrl.LastIndexOf('/') + 1);
            _cachedDriverVersion = version;
            return _cachedDriverVersion;
        }

        /// <inheritdoc />
        public override DriverOptions GetDefaultDriverOptions()
        {
            return new FirefoxOptions();
        }

        /// <inheritdoc />
        public override IWebDriver CreateWebDriver(string driverPath, DriverOptions options)
        {
            return new FirefoxDriver(driverPath, options as FirefoxOptions);
        }
    }
}