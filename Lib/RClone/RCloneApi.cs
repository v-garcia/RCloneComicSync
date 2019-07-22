using Lib.Logger;
using Lib.RClone.ApiObjects;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone
{
    public class RCloneApi : IRCloneApi
    {

        protected readonly ILogWriter _logger;

        protected readonly IRClone _rclone;

        public RCloneApi(ILogWriter logger, IRClone rclone)
        {
            _logger = logger;
            _rclone = rclone;
        }

        public async ITask CreateRemote(string name, string type)
        {
            var remotes = await GetRemotes();
            if (!remotes.Any(x => x == name))
            {
                await _rclone.JSonRcCommand("config/create", new { name = name, type = type, parameters = new { } });
            }
        }

        public async ITask<bool> CreateRemoteIfNotExists(string name, string type)
        {
            var remotes = await GetRemotes();
            if (!remotes.Any(x => x == name))
            {
                await CreateRemote(name, type);
                return true;
            }
            return false;
        }

        public async ITask<IEnumerable<String>> GetRemotes()
        {
            var resp = await _rclone.JSonRcCommand("config/listremotes");
            IEnumerable<String> remote = resp["remotes"];
            return remote;
        }

        public async ITask<IEnumerable<IJob>> GetJobs()
        {
            var ids = await GetJobsIds();
            var jobs = new List<IJob>();

            foreach (var id in ids)
            {
                var jobStatus = await GetJobStatus(id);
                jobs.Add(jobStatus);
            }

            return jobs;
        }
        public async ITask<IEnumerable<int>> GetJobsIds()
        {
            var resp = await _rclone.JSonRcCommand("job/list");
            IEnumerable<int> remote = resp["jobids"];
            return remote;
        }

        public ITask<IJob> GetJobStatus(int jobId)
        {
            return _rclone.JSonRcCommand<Job>("job/status", new { jobid = jobId });
        }

        public async ITask<int> StartCopyAsync(String sourceStr, String destStr)
        {
            var resp = await _rclone.JSonRcCommand("sync/copy", new
            {
                srcFs = sourceStr,
                dstFs = destStr,
                _async = true
            });

            int responseId = resp.jobid;

            return responseId;
        }

        public async ITask<bool> IsJobRunning(int jobId)
        {
            var jobsIds = await GetJobsIds();

            if (!jobsIds.Contains(jobId))
            {
                return false;
            }

            var jobStatus = await GetJobStatus(jobId);

            return !jobStatus.Finished;
        }
    }
}
