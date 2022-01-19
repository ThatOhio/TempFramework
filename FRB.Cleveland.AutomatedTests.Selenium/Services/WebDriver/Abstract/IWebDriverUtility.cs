using OpenQA.Selenium;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver.Abstract
{
    public interface IWebDriverUtility
    {
        /// <summary>
        /// The file name of the WebDriver.
        /// </summary>
        string WebDriverName { get; }
        /// <summary>
        /// The name of the archive to download from <see cref="GetDownloadUrl"/>.
        /// </summary>
        string OsArchiveName { get; }
        /// <summary>
        /// Gets the url that can be used to download the WebDriver.
        /// </summary>
        string GetDownloadUrl();
        /// <summary>
        /// Gets the latest version of the WebDriver.
        /// </summary>
        string GetWebDriverVersion();
        /// <summary>
        /// Creates an instance of <see cref="DriverOptions"/> that matches the appropriate browser.
        /// </summary>
        DriverOptions GetDefaultDriverOptions();
        /// <summary>
        /// Creates an instance of the appropriate <see cref="IWebDriver"/>.
        /// </summary>
        /// <param name="driverPath">File path of the physical web driver to use. If one is needed, download it from <see cref="GetDownloadUrl"/>.</param>
        /// <param name="options">The <see cref="DriverOptions"/> to provide to the new <see cref="IWebDriver"/>.</param>
        IWebDriver CreateWebDriver(string driverPath, DriverOptions options);
    }
}