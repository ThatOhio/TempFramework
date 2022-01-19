using Castle.DynamicProxy;

namespace FRB.Cleveland.AutomatedTests.Lib.Utilities.Serilog
{
    public static class LoggingHelpers
    {
        /// <summary>
        /// Registers an Interceptor that will perform logging of method calls, including parameters passed, and property access (both getting and setting).
        /// </summary>
        /// <param name="element">The instance of <see cref="IElement"/> to proxy.</param>
        /// <returns>A DynamicProxy from CastleWindsor wrapping the provided <see cref="IElement"/> instance.</returns>
        public static IElement AddLoggingProxy(this IElement element)
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithTarget(element, new SerilogInterceptor());
            return proxy;
        }

        /// <summary>
        /// Registers an Interceptor that will perform logging of method calls, including parameters passed, and property access (both getting and setting).
        /// </summary>
        /// <param name="browser">The instance of <see cref="IBrowser"/> to proxy.</param>
        /// <returns>A DynamicProxy from CastleWindsor wrapping the provided <see cref="IBrowser"/> instance.</returns>
        public static IBrowser AddLoggingProxy(this IBrowser browser)
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithTarget(browser, new SerilogInterceptor());
            return proxy;
        }
    }
}