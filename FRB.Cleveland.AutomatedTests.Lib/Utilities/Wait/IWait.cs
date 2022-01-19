using System;

namespace FRB.Cleveland.AutomatedTests.Lib.Utilities.Wait
{
    public interface IWait<TContext>
    {
        /// <summary>
        ///     Gets how long to wait for the evaluated condition to be true.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        ///     Gets how often the condition should be evaluated. The default is 500 milliseconds.
        /// </summary>
        TimeSpan PollingInterval { get; }

        /// <summary>
        ///     Configures this instance to ignore specific types of exceptions while waiting for a condition.
        ///     Any exceptions not whitelisted will be allowed to propagate, terminating the wait.
        /// </summary>
        /// <param name="exceptionTypes">The types of exceptions to ignore.</param>
        void IgnoreExceptionTypes(params Type[] exceptionTypes);

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
        TResult Until<TResult>(Func<TContext, TResult> condition);
    }
}