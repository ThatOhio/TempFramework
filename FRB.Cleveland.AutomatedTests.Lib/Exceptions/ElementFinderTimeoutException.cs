using System;

namespace FRB.Cleveland.AutomatedTests.Lib.Exceptions
{
    public class ElementFinderTimeoutException : Exception
    {
        public ElementFinderTimeoutException(ElementFinder finder, Exception lastException)
            : base(
                $"The ElementFinder timed out after {finder.Timeout.TotalSeconds} seconds. " +
                $"FindBy: {finder.FindBy.FindByType.ToString()} with Value: {finder.FindBy.Value}.", lastException)
        {
        }
    }
}