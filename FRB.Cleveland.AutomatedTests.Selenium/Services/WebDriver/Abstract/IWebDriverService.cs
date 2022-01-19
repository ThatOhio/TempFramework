using OpenQA.Selenium;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver.Abstract
{
    public interface IWebDriverService
    {
        IWebDriver GetDriver(DriverOptions options);
        DriverOptions DefaultDriverOptions { get; }
        string DownloadFolder { get; }
    }
}