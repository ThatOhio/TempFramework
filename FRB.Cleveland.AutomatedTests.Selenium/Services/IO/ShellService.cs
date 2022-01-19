using System.Diagnostics;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO
{
    public class ShellService : IShellService
    {
        /// <summary>
        /// Executes the provided command against '/bin/bash' 
        /// </summary>
        /// <param name="command">The bash command to execute, including any parameters.</param>
        /// <returns>The StandardOutput of the console.</returns>
        public string ExecuteBash(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}