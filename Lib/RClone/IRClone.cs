using MorseCode.ITask;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lib.RClone
{
    public interface IRClone
    {
        string RClonePath { get; }

        ITask<bool> IsDaemonRunning();
        ITask<dynamic> JSonRcCommand(string operation, object opParams = null);
        ITask<T> JSonRcCommand<T>(string operation, object opParams = null);
        ITask KillDaemonIfExists();
        Process ShowRCloneConfig();
        Process StartDaemon(string logFilePath, IEnumerable<string> includes, IEnumerable<string> excludes);
    }
}