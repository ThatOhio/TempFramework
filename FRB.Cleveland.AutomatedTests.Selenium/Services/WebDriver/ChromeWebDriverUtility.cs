using System;
using System.Linq;
using System.Runtime.InteropServices;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;
using FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver.Abstract;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver
{
    public class ChromeWebDriverUtility : BaseWebDriverUtility
    {
        public const string ChromePlistLocation = "/Applications/Google Chrome.app/Contents/Info.plist";
        public const string ChromePlistVersionKey = "CFBundleShortVersionString";
        public const string ChromeRegistryKey =
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
        private const string ChromeDriverUrl = "https://chromedriver.storage.googleapis.com";
        private string _cachedWebDriverVersion = "";

        private OSPlatform _osPlatform;

        public ChromeWebDriverUtility(OSPlatform osPlatform)
        : this(osPlatform, new RegistryService(), new PListService(), new WebRequestService())
        {
        }

        public ChromeWebDriverUtility(OSPlatform osPlatform, IRegistryService registryService,
            IPListService pListService, IWebRequestService webRequestService)
            : base(registryService, pListService, webRequestService)
        {
            _osPlatform = osPlatform;
        }

        /// <inheritdoc />
        public override string WebDriverName => "chromedriver";

        /// <inheritdoc />
        public override string OsArchiveName
        {
            get
            {
                string zipName;
                if (_osPlatform == OSPlatform.Windows)
                    zipName = "chromedriver_win32.zip";
                else if (_osPlatform == OSPlatform.OSX)
                    zipName = "chromedriver_mac64.zip";
                else
                    zipName = "chromedriver_linux64.zip";
                return zipName;
            }
        }

        /// <summary>
        /// Uses various methods to attain the locally installed version of Chrome.
        /// </summary>
        public string GetLocalBrowserVersion()
        {
            // This method is likely going to be fickle in the future, if it causes issues, the consuming method,
            // GetWebDriverVersion is written to handle a "" return and ignore local version.
            if (_osPlatform == OSPlatform.Windows)
                return RegistryService.GetFileVersionFromRegistry(ChromeRegistryKey);
            if (_osPlatform == OSPlatform.OSX)
                return PListService.GetVersionFromPList(ChromePlistLocation, ChromePlistVersionKey);

            return "";
        }

        /// <summary>
        /// Gets the WebDriver version that either matches the local Browser version, or latest available driver.
        /// </summary>
        public override string GetWebDriverVersion()
        {
            if (!string.IsNullOrWhiteSpace(_cachedWebDriverVersion))
                return _cachedWebDriverVersion;
            string version;

            // First attempt to use the local browser version to find a driver
            version = GetLocalBrowserVersion();

            // An empty version here generally means we could not locate the browser locally, 
            // or code has not been written to find this browser in the current OS. 
            if (string.IsNullOrWhiteSpace(version))
            {
                // Get the latest version of the driver, and return.
                version = GetChromeDriverVersion();
                _cachedWebDriverVersion = version;
                return version;
            }

            // Check the remote for a version of the web driver matching the local chrome version.
            var remoteUrl = $"{ChromeDriverUrl}/{version}/{OsArchiveName}";
            if (WebRequestService.UrlExists(remoteUrl))
            {
                // Driver exists, cache it and return. 
                _cachedWebDriverVersion = version;
                return version;
            }

            // If a remote version doesnt exist, get the latest version for this major version.
            // Chrome versions look something like ##.##.##.##, so we can split on . and get the first #
            var majorVersion = version.Split(".", StringSplitOptions.RemoveEmptyEntries).First();
            version = GetChromeDriverVersion(majorVersion);
            _cachedWebDriverVersion = version;
            return version;
        }

        /// <summary>
        /// Gets the latest version of Chrome WebDriver
        /// </summary>
        /// <param name="majorVersion">Optionally check for the latest release for the provided major version. Example: 78</param>
        public string GetChromeDriverVersion(string majorVersion = "")
        {
            if (!string.IsNullOrWhiteSpace(majorVersion))
                majorVersion = "_" + majorVersion;
            var latestVersionUrl = ChromeDriverUrl + $"/LATEST_RELEASE" + majorVersion;
            return WebRequestService.GetStringResponse(latestVersionUrl);
        }

        /// <inheritdoc />
        public override string GetDownloadUrl()
        {
            return $"{ChromeDriverUrl}/{GetWebDriverVersion()}/{OsArchiveName}";
        }

        /// <inheritdoc />
        public override DriverOptions GetDefaultDriverOptions()
        {
            return new ChromeOptions();
        }

        /// <inheritdoc />
        public override IWebDriver CreateWebDriver(string driverPath, DriverOptions options)
        {
            return new ChromeDriver(driverPath, options as ChromeOptions);
        }
    }
}