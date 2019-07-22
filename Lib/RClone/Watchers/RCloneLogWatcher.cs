using Lib.Logger;
using Lib.Tools.LiveFileReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lib.RClone.Watchers
{
    public class RCloneLogWatcher : LiveFileReader, IRCloneLogWatcher
    {
        Regex _copyFileRegex = new Regex(@"Copied( \(.*\))?$");
        public RCloneLogWatcher(ILogWriter logWriter, string filePath) : base(logWriter, filePath) { }

        public event EventHandler<IRCloneFileCopied> OnNewFile = delegate { };
        protected override void HandleNewLine(string newLine)
        {
            _logWriter.WriteLine(newLine);
            LookForNewFileCopied(newLine);
        }

        protected void LookForNewFileCopied(string newLine)
        {
            if (!_copyFileRegex.IsMatch(newLine))
            {
                return;
            }
            var splittedString = newLine.Split(':');
            var fileName = splittedString[1].Trim();

            OnNewFile(null, new RCloneFileCopied(newLine, fileName));
        }
    }


}
