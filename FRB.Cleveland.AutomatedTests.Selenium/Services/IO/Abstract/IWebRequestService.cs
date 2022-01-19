namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract
{
    public interface IWebRequestService
    {
        bool UrlExists(string url);
        void DownloadFile(string url, string savePath);
        string GetStringResponse(string url);
        string GetRequestUri(string url);
    }
}