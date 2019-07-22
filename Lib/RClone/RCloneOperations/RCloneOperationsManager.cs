using Lib.AppConfig;
using Lib.Logger;
using Lib.RClone.RCloneOperations;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone
{
    public class RCloneOperationsManager : IRCloneOperationsManager
    {
        private IList<IRCloneCopyOperation> _runningCopyOps = new List<IRCloneCopyOperation>();
        public IList<IRCloneCopyOperation> RunningCopyOps { get => _runningCopyOps; }

        protected readonly ILogWriter _log;

        protected readonly IRCloneApi _rcloneApi;


        public event EventHandler<IRCloneCopyOperation> OnNewJob = delegate { };

        public event EventHandler<IRCloneCopyOperation> OnJobEnded = delegate { };

        public RCloneOperationsManager(ILogWriter log, IRCloneApi rcloneApi)
        {
            _log = log;
            _rcloneApi = rcloneApi;
        }

        public async ITask<IEnumerable<IRCloneCopyOperation>> StartCopyMultiple(IEnumerable<(IRClonePath, IRClonePath)> operations)
        {
            List<IRCloneCopyOperation> res = new List<IRCloneCopyOperation>();
            await RefreshRunningCopyOps();

            foreach (var (from, to) in operations)
            {
                var op = await StartCopy(from, to);
                if (!(op is null))
                {
                    OnNewJob(null, op);
                    res.Add(op);
                }
            }
            return res;
        }

        public async ITask<IRCloneCopyOperation> StartCopy(IRClonePath from, IRClonePath to)
        {


            if (IsOperationInList(from, to))
            {
                _log.WriteLine($"Want to start copy from {from} to {to} but this operation is already running");
                return null;
            }

            int jobId = await _rcloneApi.StartCopyAsync(from.ToString(), to.ToString());

            var newOperation = new RCloneCopyOperation() { jobId = jobId, source = from, destination = to };

            RunningCopyOps.Add(newOperation);

            _log.WriteLine($"New job: {newOperation}");

            return newOperation;
        }

        protected bool IsOperationInList(IRClonePath from, IRClonePath to)
        {
            return RunningCopyOps.Any(x => x.source.ToString() == from.ToString() & x.destination.ToString() == to.ToString());
        }

        protected async ITask RefreshRunningCopyOps()
        {
            var stillRuning = new List<IRCloneCopyOperation>();
            var ended = new List<IRCloneCopyOperation>();

            foreach (var op in RunningCopyOps)
            {
                if (await _rcloneApi.IsJobRunning(op.jobId))
                {
                    //_log.WriteLine($"Running job : {op.ToString()}");
                    stillRuning.Add(op);
                }
                else
                {
                    _log.WriteLine($"Ended job : {op.ToString()}");
                    ended.Add(op);
                }
            }

            _runningCopyOps = stillRuning;
            ended.ForEach(e => OnJobEnded(null, e));
        }
    }


}
