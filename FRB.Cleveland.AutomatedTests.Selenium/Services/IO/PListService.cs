using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO
{
    public class PListService : IPListService
    {
        public string GetVersionFromPList(string plistFile, string key)
        {
            var plist = new PList(plistFile);
            return plist[key];
        }
    }
}