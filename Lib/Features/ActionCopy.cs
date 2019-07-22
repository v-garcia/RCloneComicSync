using Lib.AppConfig;
using Lib.Logger;
using Lib.RClone;
using Lib.RClone.Hooks;
using Lib.RClone.RCloneOperations;
using Lib.RClone.Watchers;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Lib.Features
{
    public sealed class ActionCopy : IActionCopy
    {

        private readonly ILogWriter _logWriter;

        private readonly IRCLoneOperationsManagerAutoRefreshed _opManager;

        private readonly IAppConfig _appConfig;

        private readonly IRClone _rClone;

        private readonly IRCloneLogWatcher _logWatcher;

        private readonly IAppConstants _constants;

        private readonly ICheckCopiedFileHook _checkCopiedFiles;
        public ActionCopy(ILogWriter logWriter, IRCLoneOperationsManagerAutoRefreshed opManager, IAppConfig appConfig,
                          IRCloneApi rcloneApi, IRClone rClone, IRCloneLogWatcher logWatcher, IAppConstants constants,
                          ICheckCopiedFileHook checkCopiedFiles)
        {
            _logWriter = logWriter;
            _opManager = opManager;
            _appConfig = appConfig;
            _rcloneApi = rcloneApi;
            _rClone = rClone;
            _constants = constants;
            _logWatcher = logWatcher;
            _checkCopiedFiles = checkCopiedFiles;
            InitRClonePathPairs();
            SetUpTimer();

        }

        private Timer _copyTimer;

        private readonly IRCloneApi _rcloneApi;

        private IEnumerable<(IRClonePath, IRClonePath)> _copyPaths;
        public async void Run(Action onFinish)
        {
            // Kill daemon it's already launched
            await _rClone.KillDaemonIfExists();

            // Build the exclude and includes
            var includes = new List<string>() { _appConfig.FilesToSync };

            var deletedFilePath = Path.Combine(_appConfig.WorkingFolder, _constants.DeletedFilesFileName);
            var excludes = _appConfig.ExcludeDeletedFromCopy ? File.ReadAllLines(deletedFilePath) : Enumerable.Empty<string>();

            // Start the rclone daemon
            _rClone.StartDaemon(logFilePath: _appConfig.RCloneLogPath, includes: includes, excludes:excludes); ;

            // Create local remote if not exists
            if (await _rcloneApi.CreateRemoteIfNotExists(_constants.RCloneLocalRemoteName, "local"))
            {
                _logWriter.WriteLine($"Remote '${_constants.RCloneLocalRemoteName}' was created in rclone config");
            }

            // Start watching for job status
            _opManager.StartRefresh();

            // Start reading the logs
            _logWatcher.StartWatching();

            // Check copied files
            _checkCopiedFiles.Enable();

            // Start the timer which trigger the operationss
            if (_appConfig.StartCopyEveryMinuts.HasValue)
            {
                _copyTimer.Start();
            }
            
            // Start the operation
            CopyRun();

            // On jobs end looks if the application can end
            _opManager.OnJobEnded += async (s, e) =>
            {
                if(!_appConfig.StartCopyEveryMinuts.HasValue && !_opManager.RunningCopyOps.Any())
                {
                    await Task.WhenAll(_checkCopiedFiles.RunningHooks.Select(x => x.AsTask()));
                    _logWriter.WriteLine("No more jobs to execute");
                    onFinish();
                }
            };
        }

        private void InitRClonePathPairs()
        {
            IRClonePath localDestination = (new RClonePath() { RemoteName = _constants.RCloneLocalRemoteName, Path = _appConfig.WorkingFolder });
            _copyPaths = from df in _appConfig.SourceFolders
                         select (df, localDestination);
        }

        private void SetUpTimer()
        {
            if(!_appConfig.StartCopyEveryMinuts.HasValue)
            {
                return;
            }

            _copyTimer = new Timer();
            _copyTimer.Interval = _appConfig.StartCopyEveryMinuts.Value * 1000 * 60;
            _copyTimer.AutoReset = true;
            _copyTimer.Elapsed += (s, e) => CopyRun();
        }
        private void CopyRun()
        {
            _opManager.StartCopyMultiple(_copyPaths);
        }
    }
}
