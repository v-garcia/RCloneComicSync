using System;

namespace Lib.Logger
{
    public interface ILogWriter
    {
        void WriteLine(string str);
    }
}