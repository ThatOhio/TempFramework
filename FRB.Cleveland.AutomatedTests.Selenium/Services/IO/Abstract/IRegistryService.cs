namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract
{
    public interface IRegistryService
    {
        string GetFileVersionFromRegistry(string regKey);
    }
}