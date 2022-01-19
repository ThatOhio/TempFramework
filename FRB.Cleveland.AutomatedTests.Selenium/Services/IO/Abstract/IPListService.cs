namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract
{
    public interface IPListService
    {
        string GetVersionFromPList(string plistFile, string key);
    }
}