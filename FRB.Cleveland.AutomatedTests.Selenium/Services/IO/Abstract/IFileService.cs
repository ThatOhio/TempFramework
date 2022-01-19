using System.Runtime.InteropServices;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract
{
    public interface IFileService
    {
        void ExtractArchive(string archiveFilePath, string extractTo);
        void CreateDirectory(string directory);
        void MakeFileExecutable(string filePath, OSPlatform osPlatform);
        bool FileExists(string filePath);
        string GetFileParentDirectoryName(string filePath);
    }
}