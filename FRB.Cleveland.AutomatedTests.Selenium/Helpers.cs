using System.Runtime.InteropServices;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;

namespace FRB.Cleveland.AutomatedTests.Selenium
{
    public static class Helpers
    {
        /// <exception cref="TestLibException">The current OS is not supported..</exception>
        public static OSPlatform GetCurrentOsPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSPlatform.OSX;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;
            else
                throw new TestLibException("Current OS is not supported: " + RuntimeInformation.OSDescription);
        }
    }
}