using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;
using OpenQA.Selenium;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.WebDriver.Abstract
{
    public abstract class BaseWebDriverUtility : IWebDriverUtility
    {
        protected readonly IRegistryService RegistryService;
        protected readonly IPListService PListService;
        protected readonly IWebRequestService WebRequestService;

        public BaseWebDriverUtility(IRegistryService registryService, IPListService pListService,
            IWebRequestService webRequestService)
        {
            RegistryService = registryService;
            PListService = pListService;
            WebRequestService = webRequestService;
        }

        public abstract string WebDriverName { get; }
        public abstract string OsArchiveName { get; }
        public abstract string GetDownloadUrl();
        public abstract string GetWebDriverVersion();
        public abstract DriverOptions GetDefaultDriverOptions();
        public abstract IWebDriver CreateWebDriver(string driverPath, DriverOptions options);
    }
}