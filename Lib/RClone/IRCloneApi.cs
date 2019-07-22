using System.Collections.Generic;
using System.Diagnostics;
using Lib.RClone.ApiObjects;
using MorseCode.ITask;

namespace Lib.RClone
{
    public interface IRCloneApi
    {
        ITask CreateRemote(string name, string type);
        ITask<bool> CreateRemoteIfNotExists(string name, string type);
        ITask<IEnumerable<IJob>> GetJobs();
        ITask<IEnumerable<int>> GetJobsIds();
        ITask<IJob> GetJobStatus(int jobId);
        ITask<IEnumerable<string>> GetRemotes();
        ITask<bool> IsJobRunning(int jobId);
        ITask<int> StartCopyAsync(string sourceStr, string destStr);
    }
}