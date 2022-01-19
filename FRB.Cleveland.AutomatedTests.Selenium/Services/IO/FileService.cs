using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using FRB.Cleveland.AutomatedTests.Lib.Exceptions;
using FRB.Cleveland.AutomatedTests.Selenium.Services.IO.Abstract;

namespace FRB.Cleveland.AutomatedTests.Selenium.Services.IO
{
    public class FileService : IFileService
    {
        private readonly IShellService _shellService;

        public FileService() : this(new ShellService()) { }

        public FileService(IShellService shellService)
        {
            _shellService = shellService;
        }

        /// <exception cref="TestLibException">Archive file type not supported.</exception>
        public void ExtractArchive(string archiveFilePath, string extractTo)
        {

            if (archiveFilePath.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                ZipFile.ExtractToDirectory(archiveFilePath, extractTo);
            else if (archiveFilePath.EndsWith(".tar.gz", StringComparison.InvariantCultureIgnoreCase))
            {
                var command = $"tar -C {extractTo} -xzf " + archiveFilePath;
                _shellService.ExecuteBash(command);
            }
            else
                throw new TestLibException("Archive type not supported: " + archiveFilePath);
        }

        public void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public void MakeFileExecutable(string filePath, OSPlatform osPlatform)
        {
            // This is not necessary in Windows.
            if (osPlatform == OSPlatform.Windows)
                return;

            // Only call chmod in osx and linux.
            if (osPlatform != OSPlatform.OSX && osPlatform != OSPlatform.Linux) return;
            var command = "chmod 755 " + filePath;
            _shellService.ExecuteBash(command);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string GetFileParentDirectoryName(string filePath)
        {
            return new FileInfo(filePath).DirectoryName;
        }
    }
}