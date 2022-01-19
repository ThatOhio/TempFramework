using FRB.Cleveland.AutomatedTests.Lib;
using FRB.Cleveland.AutomatedTests.Lib.Utilities.Serilog;

namespace FRB.Cleveland.AutomatedTests.Selenium
{
    public abstract class SeleniumTest : BaseLibTest
    {
        /// <summary>
        /// Provides easy access to an already running browser to any given <see cref="SeleniumTest"/>.
        /// </summary>
        public IBrowser Browser { get; protected set; }

        /// <inheritdoc />
        protected SeleniumTest(LibFixture fixture) : base(fixture)
        {
            Browser = new SeleniumBrowser().AddLoggingProxy();
        }

        /// <summary>
        /// Disposes of the current <see cref="Browser"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Browser.Dispose();
                Browser = null;
            }
            base.Dispose(disposing);
        }
    }
}