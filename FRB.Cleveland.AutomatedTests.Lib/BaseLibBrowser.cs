using System;
using System.Collections.Generic;

namespace FRB.Cleveland.AutomatedTests.Lib
{
    public abstract class BaseLibBrowser : IBrowser
    {
        public abstract string CurrentUrl { get; }
        public abstract string PageSource { get; }
        public abstract string BrowserType { get; }
        public abstract Type ElementNotFoundException { get; }

        /// <summary>
        /// Performs any startup logic of the concrete <see cref="BaseLibBrowser"/>.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Navigates to the provided url, waiting for the primary dom to load before returning.
        /// </summary>
        /// <param name="url">The url you would like to navigate to.</param>
        /// <returns>The amount of time it took for the page to load.</returns>
        public abstract TimeSpan Navigate(string url);

        /// <summary>
        /// Closes the current browser and performs any tear down logic.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Uses the <see cref="ElementFinder"/> to search for an element in the <see cref="BaseLibBrowser"/>.
        /// </summary>
        /// <param name="finder">An instance of <see cref="ElementFinder"/> to use to perform an element search.</param>
        public abstract IElement GetElement(FindBy findBy);

        /// <summary>
        /// Uses the <see cref="ElementFinder"/> to search for any elements in the <see cref="BaseLibBrowser"/>.
        /// </summary>
        /// <param name="finder">An instance of <see cref="ElementFinder"/> to use to perform the element search.</param>
        public abstract IEnumerable<IElement> GetElements(FindBy findBy);

        /// <summary>
        /// Creates a new <see cref="ElementFinder"/> using the current <see cref="BaseLibBrowser"/>.
        /// </summary>
        /// <param name="findBy">The primary <see cref="FindBy"/> condition to use to perform the search.</param>
        /// <returns></returns>
        public virtual ElementFinder ElementFinder(FindBy findBy)
        {
            return new ElementFinder(findBy, this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Close();
        }
    }
}