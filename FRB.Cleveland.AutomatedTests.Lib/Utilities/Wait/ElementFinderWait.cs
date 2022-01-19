using System;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;

namespace FRB.Cleveland.AutomatedTests.Lib.Utilities.Wait
{
    /// <inheritdoc />
    public class ElementFinderWait : Wait<ElementFinder>
    {
        /// <inheritdoc />
        public ElementFinderWait(ElementFinder finder, TimeSpan timeout) : base(finder,
            timeout, TimeSpan.FromMilliseconds(500))
        {
        }

        /// <summary>
        ///     Throws an <see cref="ElementFinderTimeoutException" />.
        /// </summary>
        /// <exception cref="ElementFinderTimeoutException"></exception>
        protected override void ThrowTimeoutException(ElementFinder context, Exception lastException)
        {
            throw new ElementFinderTimeoutException(context, lastException);
        }
    }
}