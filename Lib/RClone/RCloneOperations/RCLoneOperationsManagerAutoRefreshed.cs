using Lib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Lib.RClone.RCloneOperations
{
    public class RCLoneOperationsManagerAutoRefreshed : RCloneOperationsManager, IRCLoneOperationsManagerAutoRefreshed
    {
        protected readonly Timer _timer;

        public RCLoneOperationsManagerAutoRefreshed(ILogWriter log, IRCloneApi rcloneApi, int secondsBetweenRefresh) : base(log, rcloneApi)
        {
            _timer = new Timer(secondsBetweenRefresh * 1000);
            _timer.AutoReset = true;
            _timer.Elapsed += (o, e) =>
            {
                this.RefreshRunningCopyOps();
            };
        }

        public void StartRefresh() => _timer.Start();

        public void StopRefresh() => _timer.Stop();

    }
}
