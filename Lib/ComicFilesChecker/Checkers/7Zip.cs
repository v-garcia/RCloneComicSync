using Lib.Logger;
using MorseCode.ITask;
using RunProcessAsTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.ComicFilesChecker.Checkers
{
#pragma warning disable IDE1006 // Naming Styles
    public class _7Zip : FileChecker, IFileChecker, I_7Zip
#pragma warning restore IDE1006 // Naming Styles
    {

        public override string Name => "7zip";

        private ILogWriter _log;

        public _7Zip(ILogWriter log, string exePath) : base(exePath)
        {
            _log = log;
        }

        public override bool CanCheckFile(string fileName)
        {
            var ext = Path.GetExtension(fileName);

            return ext == ".cbz" || ext == ".cbr";
        }

        public async override ITask<bool> CheckFilePath(string fileName)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = _exePath;
            startInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
            startInfo.Arguments = $" t \"{Path.GetFileName(fileName)}\"";
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            var res = await ProcessEx.RunAsync(startInfo);


            if (res.ExitCode == 0)
            {
                _log.WriteLine($"7zip successfully checked '{Path.GetFileName(fileName)}'");
                return true;
            }

            if (res.ExitCode == 1)
            {
                _log.WriteLine($"7zip on file '{Path.GetFileName(fileName)}' has warnings");
                return true;
            }

            _log.WriteLine($"7zip on file '{Path.GetFileName(fileName)}' has failled with code {res.ExitCode}");
            return false;
        }


    }
}