using Lib.Tools.LiveFileReader;
using System;

namespace Lib.RClone.Watchers
{
    public interface IRCloneLogWatcher: ILiveFileReader
    {
        event EventHandler<IRCloneFileCopied> OnNewFile;
    }
}