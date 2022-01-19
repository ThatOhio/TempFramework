using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using FRB.Cleveland.AutomatedTests.Lib.Attributes;
using Serilog;

namespace FRB.Cleveland.AutomatedTests.Lib.Utilities.Serilog
{
    /// <summary>
    /// We utilize CastleWindsor as an IoC container, allowing us to dynamically log method calls (and the parameters passed to them),
    /// without the need to manually apply logging on all of those methods.
    /// </summary>
    public class SerilogInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var converter = new SerilogMessageConverter(invocation);
            var sw = Stopwatch.StartNew();
            invocation.Proceed();
            sw.Stop();
            if (converter.IgnoreLogging)
                return;
            converter.Parameters.Add(sw);
            Log.Information(converter.Template + " executed in {@ExecutionTime}", converter.Parameters.ToArray());
        }

        private class SerilogMessageConverter
        {
            public SerilogMessageConverter(IInvocation invocation)
            {
                // First we check if the method being called has been decorated with LogIgnore.
                var logIgnoreAttributes = invocation.Method.GetCustomAttributes(typeof(LogIgnoreAttribute), true);
                if (logIgnoreAttributes.Any())
                {
                    IgnoreLogging = true;
                    return;
                }

                // We then construct the message template to be used by Serilog,
                // if the method has no parameters, we are done.
                Parameters = new List<object>();
                Parameters.Add(invocation.Method.Name);
                var sb = new StringBuilder();
                sb.Append("Method call named: {methodName}");
                if (!invocation.Arguments.Any())
                {
                    Template = sb.ToString();
                    return;
                }

                // When the method has parameters, we need to place-hold them in the message template,
                // we also keep a list of each parameter value to be passed to Serilog.
                sb.Append(" with parameters: ");
                var parameters = invocation.Method.GetParameters();
                for (var i = 0; i < invocation.Arguments.Length; i++)
                {
                    var argument = invocation.Arguments[i];
                    var parameter = parameters[i];

                    Parameters.Add(argument);
                    if (argument.GetType() == typeof(ElementFinder))
                        sb.AppendLine("{@" + parameter.Name + "}");
                    else
                        sb.AppendLine("{" + parameter.Name + "}");
                }

                Template = sb.ToString();
            }

            public string Template { get; }
            public List<object> Parameters { get; }
            public bool IgnoreLogging { get; }
        }
    }
}