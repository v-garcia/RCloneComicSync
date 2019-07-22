using System;

namespace Lib.Tools.LiveFileReader
{
    public interface ILiveFileReader
    {
        event EventHandler<string> OnNewLine;
        void StartWatching();
        void StopWatching();
    }
}