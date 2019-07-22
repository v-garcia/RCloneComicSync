using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone.Watchers
{
    public interface IRCloneFileCopied
    {
        string FileName { get; }
        string Text { get; }
    }

    public class RCloneFileCopied : IRCloneFileCopied
    {

        public RCloneFileCopied(string s, string fileName) { Text = s; FileName = fileName; }

        public String Text { get; }

        public String FileName { get; }

    }
}
