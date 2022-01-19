using System.Net;
using System.Net.Http;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO
{
    public class WebRequestService : IWebRequestService
    {
        /// <exception cref="WebException">Any non 404 exception is not handled and will be thrown.</exception>
        public bool UrlExists(string url)
        {
            try
            {
                // Attempt to get a response from the Url
                var req = WebRequest.Create(url);
                req.GetResponse();
                // If the above line doesn't 404, we have a valid url. 
                return true;
            }
            catch (WebException ex)
            {
                // If the exception is not related to a 404, throw. 
                if (!ex.Message.Contains("(404) Not Found")) throw;
                return false; // The error was a 404.
            }
        }

        public void DownloadFile(string url, string savePath)
        {
            var webClient = new WebClient();
            webClient.DownloadFile(url, savePath);
        }

        public string GetStringResponse(string url)
        {
            var webClient = new HttpClient();
            var response = webClient.GetAsync(url).GetAwaiter().GetResult();
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public string GetRequestUri(string url)
        {
            var webClient = new HttpClient();
            var response = webClient.GetAsync(url).GetAwaiter().GetResult();
            return response.RequestMessage.RequestUri.ToString();
        }
    }
}