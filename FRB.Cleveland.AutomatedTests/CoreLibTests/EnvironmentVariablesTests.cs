using FluentAssertions;
using FRB.Cleveland.AutomatedTests.Lib;
using Xunit;

namespace FRB.Cleveland.AutomatedTests.CoreLibTests
{
    public class EnvironmentVariablesTests
    {
        [Fact]
        public void StaticConstructorHappyPath()
        {
            EnvironmentVariables.DockerUrl.Should().NotBeNullOrWhiteSpace();
            EnvironmentVariables.BrowserType.Should().NotBe(null);
            EnvironmentVariables.DriverType.Should().NotBe(null);
        }
    }
}