using Lib.AppConfig;
using Lib.Logger;
using Lib.Tools.HardLinks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.ComicFiles
{
    public class ComicFileManager : IComicFileManager
    {

        protected readonly string _workingDirectory;

        protected readonly string _destinationDirectory;

        protected readonly ILogWriter _logWriter;

        protected readonly IHardLinks _hardLinks;

        public readonly IAppConstants _constants;
        protected string _linkedFilesPath => Path.Combine(_workingDirectory, _constants.LinkedFilesFileName);
        protected string _deletedFilesPath => Path.Combine(_workingDirectory, _constants.DeletedFilesFileName);

        public ComicFileManager(string workingDirectory, string destinationDirectory, ILogWriter logWriter, IHardLinks hardLinks, IAppConstants constants)
        {
            _workingDirectory = workingDirectory;
            _destinationDirectory = destinationDirectory;
            _constants = constants;
            _logWriter = logWriter;
            _hardLinks = hardLinks;
        }

        public void LinkFileToDestinationDir(string relativePath)
        {
            string from = Path.Combine(_workingDirectory, relativePath);
            string to = Path.Combine(_destinationDirectory, relativePath);
            string directoryDest = Path.GetDirectoryName(to);

            if (!Directory.Exists(directoryDest))
            {
                Directory.CreateDirectory(directoryDest);
            }

            _hardLinks.CreateHardLink(from: from, to: to);

             File.AppendAllLines(_linkedFilesPath, new List<string>() { relativePath });
        }

        public void ScanForDeletedFiles()
        {
            var linkedFilesList = File.ReadAllLines(_linkedFilesPath).ToList();
            var unlinkedFiles = GetUnlinkedFiles(linkedFilesList);
            var rcloneExcludeEntries = new List<string>();

            _logWriter.WriteLine($"There are {unlinkedFiles.Count()} files to remove in the working directory");
            try
            {
                foreach (var f in unlinkedFiles)
                {
                    _logWriter.WriteLine($"Removing file '{f}' from working directory");

                    var wdPath = Path.Combine(_workingDirectory, f);

                    File.Delete(wdPath);

                    rcloneExcludeEntries.Add(f);

                    linkedFilesList.Remove(f);
                }
            }
            finally
            {
                File.AppendAllLines(_deletedFilesPath, rcloneExcludeEntries);
                File.WriteAllLines(_linkedFilesPath, linkedFilesList);
            }
        }

        private IEnumerable<String> GetUnlinkedFiles(IEnumerable<string> files)
        {

            var result = new List<string>();

            foreach (var f in files)
            {
                var wdPath = Path.Combine(_workingDirectory, f);
                var rcloneExclude = new List<String>();


                if (!File.Exists(wdPath))
                {
                    _logWriter.WriteLine($"File '{f}' disappeared from working dir");
                    continue;
                }

                var linksNumber = _hardLinks.GetOtherHardLinksCount(wdPath);

                if (linksNumber == 0)
                {
                    result.Add(f);
                }

            }

            return result;
        }
    }
}
