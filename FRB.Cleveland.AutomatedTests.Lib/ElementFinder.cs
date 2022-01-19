using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.DynamicProxy;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;
using FRB.Cleveland.AutomatedTests.Lib.Utilities.Wait;
using Serilog;

namespace FRB.Cleveland.AutomatedTests.Lib
{
    public class ElementFinder
    {
        private readonly IBrowser _browser;
        private readonly List<Type> _ignoredExceptions = new List<Type>();
        private readonly List<Func<IElement, bool>> _conditions = new List<Func<IElement, bool>>();

        /// <summary>
        /// Gets the <see cref="FindBy"/> that will be used to perform the initial lookup.
        /// </summary>
        public FindBy FindBy { get; }

        /// <summary>
        /// Used to determine if wait logic should be performed when executing <see cref="GetElement"/> or <see cref="GetElements"/>.
        /// </summary>
        public bool DoWait { get; private set; }

        /// <summary>
        /// Used to determine if the <see cref="ElementFinder"/> should verify the Visibility of the element.
        /// </summary>
        public bool CheckVisibility { get; private set; }

        /// <summary>
        /// Used to determine if the <see cref="ElementFinder"/> should verify that the element is Enabled.
        /// </summary>
        public bool CheckEnabled { get; private set; }

        /// <summary>
        /// Used in <see cref="GetElements"/> to only return when the expected count of elements is found. If this value is -1, we do not validate the count of elements.
        /// </summary>
        public int ExpectedCount { get; private set; }

        /// <summary>
        /// How long to wait before timing out if <see cref="DoWait"/> is true.
        /// </summary>
        public TimeSpan Timeout { get; private set; }

        /// <summary>
        /// Gets a read only list of the registered Exception <see cref="Type"/>'s to ignore when searching for the element.
        /// </summary>
        public IReadOnlyCollection<Type> IgnoredExceptions => _ignoredExceptions.AsReadOnly();

        /// <summary>
        /// Gets a read only list of delegates that will be used to validate any found element.
        /// </summary>
        public IReadOnlyCollection<Func<IElement, bool>> Conditions => _conditions.AsReadOnly();

        /// <summary>
        /// Creates a new instance of <see cref="ElementFinder"/> with the specified <see cref="FindBy"/> and <see cref="IBrowser"/>.
        /// </summary>
        /// <param name="findBy">The <see cref="FindBy"/> to be used when searching for an element.</param>
        /// <param name="browser">The <see cref="IBrowser"/> to use to perform the initial lookup.</param>
        public ElementFinder(FindBy findBy, IBrowser browser)
        {
            FindBy = findBy ?? throw new ArgumentNullException(nameof(findBy), "The FindBy cannot be null.");
            _browser = browser ?? throw new ArgumentNullException(nameof(browser), "The IBrowser cannot be null.");

            CheckVisibility = true;
            CheckEnabled = true;
            DoWait = true;
            ExpectedCount = -1;
            Timeout = TimeSpan.FromSeconds(EnvironmentVariables.DefaultTimeoutInSeconds);
        }

        /// <summary>
        /// Sets <see cref="DoWait"/> to false, instructing the <see cref="ElementFinder"/> to not perform any wait logic.
        /// </summary>
        public ElementFinder SkipWait()
        {
            DoWait = false;
            return this;
        }

        /// <summary>
        /// Sets <see cref="CheckVisibility"/> to false, causing the <see cref="ElementFinder"/> to ignore the visibility of any found element.
        /// </summary>
        public ElementFinder SkipVisibility()
        {
            CheckVisibility = false;
            return this;
        }

        /// <summary>
        /// Sets <see cref="CheckEnabled"/> to false, causing the <see cref="ElementFinder"/> to skip checking that the element is enabled.
        /// </summary>
        /// <returns></returns>
        public ElementFinder SkipEnableCheck()
        {
            CheckEnabled = false;
            return this;
        }

        /// <summary>
        /// Sets <see cref="Timeout"/>, which is used to instruct the internal <see cref="ElementFinderWait"/> how long to wait before throwing a timeout.
        /// </summary>
        /// <param name="timeoutDuration">How long to wait before throwing a timeout.</param>
        public ElementFinder SetTimeout(TimeSpan timeoutDuration)
        {
            Timeout = timeoutDuration;
            return this;
        }

        /// <summary>
        /// Sets <see cref="ExpectedCount"/>, which is used with <see cref="GetElements"/> to verify the number of valid elements.
        /// </summary>
        /// <param name="count">The number of expected elements matching all the conditions of this <see cref="ElementFinder"/>.</param>
        public ElementFinder ExpectCount(int count)
        {
            if (count <= 0 && count != -1) throw new ArgumentOutOfRangeException(nameof(count), "Count should be a value greater than 0, or -1 to disable.");
            ExpectedCount = count;
            return this;
        }

        /// <summary>
        /// When <see cref="GetElement"/> or <see cref="GetElements"/> is called, any Exception Types passed to this method will be ignored while searching.
        /// </summary>
        /// <param name="exceptionType">A <see cref="Type"/> of <see cref="Exception"/> to ignore. Must be a <see cref="Type"/> assignable from <see cref="Exception"/>.</param>
        public ElementFinder IgnoreException(Type exceptionType)
        {
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
                throw new ArgumentException("Exception Type to be ignored must derive from System.Exception", nameof(exceptionType));

            if (!_ignoredExceptions.Contains(exceptionType))
                _ignoredExceptions.Add(exceptionType);
            return this;
        }

        /// <summary>
        /// Registers a delegate condition to be validated against any potential <see cref="IElement"/> returned by either <see cref="GetElement"/> or <see cref="GetElements"/>.
        /// </summary>
        /// <param name="condition">A conditional function used to validate each <see cref="IElement"/>.</param>
        public ElementFinder WithCondition(Func<IElement, bool> condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");

            _conditions.Add(condition);
            return this;
        }

        /// <summary>
        /// Verifies that the configured pre-conditions are valid for a given <see cref="IElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="IElement"/> to validate.</param>
        public bool IsElementValid(IElement element)
        {
            if (element == null)
                return false;

            var validity = new List<bool>() { true };

            // Our logging middleware intercepts calls to proxied instances (primarily IElement and IBrowser)
            // to avoid unnecessary noise in our logging, we circumvent the proxy here.
            var unproxiedElement = element;
            if (ProxyUtil.IsProxy(element))
                unproxiedElement = (IElement)ProxyUtil.GetUnproxiedInstance(element);
            // In some of my unit tests the above code was unwrapping a Moq proxy, so this will ensure that
            // unproxiedElement is at least not null in the event the above code has unintended side effects.
            if (unproxiedElement == null)
                unproxiedElement = element;

            if (CheckEnabled)
                validity.Add(unproxiedElement.IsEnabled);
            if (CheckVisibility)
                validity.Add(unproxiedElement.IsVisible);
            if (_conditions.Any())
                validity.Add(_conditions.All(condition => condition(unproxiedElement)));

            return validity.All(x => x);
        }

        /// <summary>
        /// Sets <see cref="ExpectedCount"/> to 1 and uses <see cref="GetElements"/>, returning when a single element is found.
        /// </summary>
        /// <exception cref="ElementFinderTimeoutException">If the configured <see cref="Timeout"/> expires.</exception>
        public IElement GetElement()
        {
            ExpectedCount = 1;
            return GetElements().First();
        }

        /// <summary>
        /// Uses an internal <see cref="ElementFinderWait"/> with the <see cref="IBrowser"/> to wait for any <see cref="IElement"/>'s matching the configured conditions to exist.
        /// </summary>
        /// <exception cref="ElementFinderTimeoutException">If the configured <see cref="Timeout"/> expires.</exception>
        public IEnumerable<IElement> GetElements()
        {
            var sw = Stopwatch.StartNew();
            Exception exception = null;
            var elementsFound = 0;
            try
            {
                if (DoWait)
                    IgnoreException(_browser.ElementNotFoundException);

                var wait = new ElementFinderWait(this, Timeout);
                if (_ignoredExceptions.Any())
                    wait.IgnoreExceptionTypes(_ignoredExceptions.ToArray());
                return wait.Until(x => FindAllValidElements(ref elementsFound));
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                sw.Stop();
                var logTemplate = "GetElement called with {@Finder}, executed in {@ExecutionTimer}, finding {ElementsFound}";
                if (exception == null)
                    Log.Information(logTemplate, this, sw, elementsFound);
                else
                    Log.Information(logTemplate + ", and threw {@Exception}", this, sw, elementsFound, exception);
            }
        }

        private IEnumerable<IElement> FindAllValidElements(ref int elementsFound)
        {
            var elements = _browser.GetElements(FindBy);

            var validElements = elements.Where(x => IsElementValid(x)).ToList();
            elementsFound = validElements.Count;

            // ExpectedCount of -1 means we do not care how many are found, only that elements are found.
            if (elementsFound > 0 && ExpectedCount == -1)
                return validElements;

            if (elementsFound == ExpectedCount)
                return validElements;

            return null;
        }
    }
}