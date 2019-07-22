using Lib.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class ConsoleLogWriter : LogWriter, ILogWriter
    {
        public override void WriteLine(string str)
        {
            Console.WriteLine(str);
            Debug.WriteLine(str);
        }
    }
}
