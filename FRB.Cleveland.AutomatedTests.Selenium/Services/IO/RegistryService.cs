using System.Diagnostics;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;
using Microsoft.Win32;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO
{
    public class RegistryService : IRegistryService
    {
        /// <exception cref="TestLibException">Could not find chrome path in the registry.</exception>
        public string GetFileVersionFromRegistry(string regKey)
        {
            var filePath = Registry.GetValue(regKey, "", null);
            if (filePath == null)
                throw new TestLibException("Could not find chrome path in the registry.");
            return FileVersionInfo.GetVersionInfo(filePath.ToString()).FileVersion;
        }
    }
}