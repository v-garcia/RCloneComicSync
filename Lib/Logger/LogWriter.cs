using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Logger
{
    public abstract class LogWriter : ILogWriter
    {
        public abstract void WriteLine(String str);
    }
}

