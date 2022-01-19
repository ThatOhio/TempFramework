using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace FRB.Cleveland.AutomatedTests.Lib.Utilities.Wait
{
    public abstract class Wait<TContext> : IWait<TContext>
    {
        private readonly TContext _context;

        private readonly List<Type> ignoredExceptions = new List<Type>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Wait{TContext}" /> class.
        /// </summary>
        /// <param name="context">The context class to store and pass through to the conditional expression.</param>
        /// <param name="timeout">How long to wait before throwing with <see cref="ThrowTimeoutException" />.</param>
        /// <param name="pollingInterval">How often to evaluate the condition.</param>
        protected Wait(TContext context, TimeSpan timeout, TimeSpan pollingInterval)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), "The context cannot be null.");

            _context = context;
            Timeout = timeout;
            PollingInterval = pollingInterval;
        }

        /// <summary>
        ///     Gets how long to wait for the evaluated condition to be true.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        ///     Gets how often the condition should be evaluated. The default is 500 milliseconds.
        /// </summary>
        public TimeSpan PollingInterval { get; }

        /// <summary>
        ///     Configures this instance to ignore specific types of exceptions while waiting for a condition.
        ///     Any exceptions not whitelisted will be allowed to propagate, terminating the wait.
        /// </summary>
        /// <param name="exceptionTypes">The types of exceptions to ignore.</param>
        public void IgnoreExceptionTypes(params Type[] exceptionTypes)
        {
            if (exceptionTypes == null)
                throw new ArgumentNullException(nameof(exceptionTypes), "exceptionTypes cannot be null.");

            foreach (var exceptionType in exceptionTypes)
                if (!typeof(Exception).IsAssignableFrom(exceptionType))
                    throw new ArgumentException("All types to be ignored must derive from System.Exception",
                        nameof(exceptionTypes));

            ignoredExceptions.AddRange(exceptionTypes);
        }

        /// <summary>
        ///     Repeatedly applies the <see cref="TContext" /> value to the given function until one of the following occurs:
        ///     <para>
        ///         <list type="bullet">
        ///             <item>The function returns neither null or false.</item>
        ///             <item>The function throws an exception that is not in the list of ignored exception types.</item>
        ///             <item>The timeout expires.</item>
        ///         </list>
        ///     </para>
        /// </summary>
        /// <typeparam name="TResult">The delegate's expected return type.</typeparam>
        /// <param name="condition">
        ///     A delegate taking an object of type <see cref="TContext" /> as its parameter, and returning
        ///     <see cref="TResult" />.
        /// </param>
        /// <returns>The delegate's return value.</returns>
        public TResult Until<TResult>(Func<TContext, TResult> condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition), "The condition cannot be null.");

            var resultType = typeof(TResult);
            if (resultType.IsValueType && resultType != typeof(bool) || !typeof(object).IsAssignableFrom(resultType))
                throw new ArgumentException("Can only wait on an object or boolean, tried to use type: " + resultType,
                    nameof(condition));

            Exception lastException = null;
            var sw = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    // Evaluate the condition
                    var result = condition(_context);

                    // Check if we are dealing with a boolean TResult, and if it is true.
                    if (IsBoolAndTrue(result))
                        return result;

                    // If it is not a boolean, check if we have a value to return.
                    if (result != null)
                        return result;
                }
                catch (Exception e)
                {
                    if (!IsIgnoredException(e))
                        throw;

                    lastException = e;
                }

                if (sw.Elapsed >= Timeout)
                    ThrowTimeoutException(_context, lastException);

                Thread.Sleep(PollingInterval);
            }
        }

        private bool IsBoolAndTrue<TResult>(TResult result)
        {
            return result is bool asBool && asBool;
        }

        protected abstract void ThrowTimeoutException(TContext context, Exception lastException);

        private bool IsIgnoredException(Exception exception)
        {
            return ignoredExceptions.Any(type => type.IsInstanceOfType(exception));
        }
    }
}