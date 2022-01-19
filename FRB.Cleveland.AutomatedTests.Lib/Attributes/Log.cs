using System;

namespace FRB.Cleveland.AutomatedTests.Lib.Attributes
{
    /// <summary>
    ///     Tells CastleWindsor that this Class/Interface should be logged via the SerilogInterceptor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class LogAttribute : Attribute
    {
    }
}