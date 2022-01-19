using System;

namespace FRB.Cleveland.AutomatedTests.Lib.Attributes
{
    /// <summary>
    ///     Can be used in conjunction with a Class/Interface decorated with Log to ignore a specific method when logging.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LogIgnoreAttribute : Attribute
    {
    }
}