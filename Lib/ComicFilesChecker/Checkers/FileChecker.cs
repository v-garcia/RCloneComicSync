using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.ComicFilesChecker.Checkers
{
    public abstract class FileChecker : IFileChecker
    {
        public abstract string Name { get; }

        protected readonly string _exePath;
        public FileChecker(string exePath)
        {
            _exePath = exePath;
        }
        public abstract bool CanCheckFile(string fileName);

        public abstract ITask<bool> CheckFilePath(string fileName);
    }
}
