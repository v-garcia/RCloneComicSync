using Lib.ComicFilesChecker.Checkers;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.ComicFilesChecker.ComicFileVerificationManager
{
    public class ComicFileCheck : IComicFileCheck
    {
        protected readonly IEnumerable<IFileChecker> _checkers;

        protected readonly string _workingDirectory;
        public ComicFileCheck(IEnumerable<IFileChecker> checkers, string workingDirectory)
        {
            _checkers = checkers;
            _workingDirectory = workingDirectory;
        }

        public async ITask<bool> CheckFile(String relativePath)
        {
            var filePath = Path.Combine(_workingDirectory, relativePath);

            var resTasks = from c in _checkers
                           where c.CanCheckFile(filePath)
                           select c.CheckFilePath(filePath).AsTask();

            var res = await Task.WhenAll(resTasks);

            return res.All(x => x);
        }
    }
}
