using Lib.Logger;
using Lib.RClone.Watchers;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone.Hooks
{
    public abstract class NewFileCopiedHook : Hook<IRCloneFileCopied>, INewFileCopiedHook
    {
        protected readonly IRCloneLogWatcher _logWatcher;

        protected ILogWriter _logWriter;

        public NewFileCopiedHook(IRCloneLogWatcher logWatcher, ILogWriter logWriter): base()
        {
            _logWatcher = logWatcher;
            _logWriter = logWriter;
        }

        public override void Disable()
        {
            _logWatcher.OnNewFile -= _logWatcher_OnNewFile;
        }

        public override void Enable()
        {
            _logWatcher.OnNewFile += _logWatcher_OnNewFile;
        }

        private void _logWatcher_OnNewFile(object sender, IRCloneFileCopied e)
        {
            Handler(e);
        }

    }

}
