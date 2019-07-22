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
    public class PDFInfo : FileChecker, IFileChecker, IPDFInfo
    {

        public override string Name => "PDFInfo";

        private ILogWriter _log;

        public PDFInfo(ILogWriter log, string exePath) : base(exePath)
        {
            _log = log;
        }

        public override bool CanCheckFile(string fileName)
        {
            var ext = Path.GetExtension(fileName);

            return ext == ".pdf";
        }

        public override async ITask<bool> CheckFilePath(string fileName)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = _exePath;
            startInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
            startInfo.Arguments = $" \"{Path.GetFileName(fileName)}\"";
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            var res = await ProcessEx.RunAsync(startInfo);

            if(res.ExitCode != 0)
            {
                _log.WriteLine($"PDFInfo successfully checked '{Path.GetFileName(fileName)}'");

                return false;
            }

            var firstLineErrOutput = res.StandardError.First() ;

            if(!string.IsNullOrEmpty(firstLineErrOutput))
            {
                _log.WriteLine($"PDFInfo on file '{Path.GetFileName(fileName)}' has error output (1st line comes next)");
                _log.WriteLine($"PDFInfo error: '{firstLineErrOutput}'");
                return false;
            }

            _log.WriteLine($"PDFInfo successfully checked '{Path.GetFileName(fileName)}'");
            return true;
        }
    }
}
