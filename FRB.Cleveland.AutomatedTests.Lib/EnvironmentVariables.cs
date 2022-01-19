using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using FRB.Cleveland.AutomatedTests.Lib.Abstract;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace FRB.Cleveland.AutomatedTests.Lib
{
    public static class EnvironmentVariables
    {
        static EnvironmentVariables()
        {
            // Load the configuration from an embedded resource rather than directly from the file.
            IConfigurationRoot configuration;
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            var resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("libConfig.json"));
            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();
            }

            // Removing configuration from raw file and including it as a resource stream.
            Enum.TryParse(configuration["DriverType"], out DriverType driverType);
            DriverType = driverType;
            Enum.TryParse(configuration["BrowserType"], out BrowserType browserType);
            BrowserType = browserType;
            DockerUrl = configuration["DockerURL"];
            MachineName = Environment.MachineName;
            UserName = Environment.UserName;
            DefaultTimeoutInSeconds = int.Parse(configuration["DefaultTimeoutInSeconds"]);
            OSPlatform = GetCurrentOsPlatform();

            InitLogging(configuration);
        }

        public static DriverType DriverType { get; }
        public static BrowserType BrowserType { get; }
        public static string DockerUrl { get; }
        public static string MachineName { get; }
        public static string UserName { get; }
        public static int DefaultTimeoutInSeconds { get; }
        public static OSPlatform OSPlatform { get; }

        private static OSPlatform GetCurrentOsPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSPlatform.OSX;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;
            throw new TestLibException("Current OS is not supported: " + RuntimeInformation.OSDescription);
        }

        private static void InitLogging(IConfigurationRoot configuration)
        {
            var logger = new LoggerConfiguration()
                    // Additional Sinks go here.
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .Enrich.WithProperty("Session", Guid.NewGuid())
                ;

            Log.Logger = logger.CreateLogger();

            Log.Information(
                "Environment Loaded: {DriverType}, {BrowserType}, {DockerURL}, {OSPlatform}, {UserName}, {MachineName}",
                DriverType, BrowserType, DockerUrl, OSPlatform, UserName, MachineName);
        }
    }
}