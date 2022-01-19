using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FRB.Cleveland.AutomatedTests.Lib;
using FRB.Cleveland.AutomatedTests.Lib.Abstract;
using FRB.Cleveland.AutomatedTests.Lib.Utilities.Serilog;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace FRB.Cleveland.AutomatedTests.Selenium
{
    public sealed class SeleniumBrowser : BaseLibBrowser
    {
        private readonly BrowserType _browserType;
        private readonly DriverType _driverType;
        private IWebDriver _driver;
        private ICapabilities _capabilities;

        internal SeleniumBrowser()
        {
            _browserType = EnvironmentVariables.BrowserType;
            _driverType = EnvironmentVariables.DriverType;
            Initialize();
        }

        public override string CurrentUrl => _driver.Url;
        public override string PageSource => _driver.PageSource;
        public override string BrowserType => _browserType.ToString();
        public override Type ElementNotFoundException => typeof(NotFoundException);

        /// <inheritdoc />
        public override void Initialize()
        {
            var driverService = new Services.WebDriver.WebDriverService(_browserType);
            var options = driverService.DefaultDriverOptions;

            _driver = _driverType switch
            {
                DriverType.Local => driverService.GetDriver(options),
                DriverType.Docker => new RemoteWebDriver(new Uri(EnvironmentVariables.DockerUrl), options),
                _ => _driver
            };
            _driver.Manage().Window.Maximize();
            _capabilities = ((RemoteWebDriver)_driver).Capabilities;
        }

        /// <inheritdoc />
        public override TimeSpan Navigate(string url)
        {
            var sw = Stopwatch.StartNew();
            _driver.Navigate().GoToUrl(url);
            sw.Stop();
            return sw.Elapsed;
        }

        /// <inheritdoc />
        public override void Close()
        {
            _driver.Quit();
        }

        /// <inheritdoc />
        public override IElement GetElement(FindBy findBy)
        {
            var element = new SeleniumElement(_driver.FindElement(findBy.ToBy()));
            return element.AddLoggingProxy();
        }

        /// <inheritdoc />
        public override IEnumerable<IElement> GetElements(FindBy findBy)
        {
            var elements = _driver.FindElements(findBy.ToBy());
            return elements.Select(x => new SeleniumElement(x).AddLoggingProxy());
        }
    }
}