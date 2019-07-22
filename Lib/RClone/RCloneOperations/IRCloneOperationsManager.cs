using System;
using System.Collections.Generic;
using Lib.RClone.RCloneOperations;
using MorseCode.ITask;

namespace Lib.RClone
{
    public interface IRCloneOperationsManager
    {
        IList<IRCloneCopyOperation> RunningCopyOps { get; }

        event EventHandler<IRCloneCopyOperation> OnJobEnded;
        event EventHandler<IRCloneCopyOperation> OnNewJob;

        ITask<IRCloneCopyOperation> StartCopy(IRClonePath from, IRClonePath to);
        ITask<IEnumerable<IRCloneCopyOperation>> StartCopyMultiple(IEnumerable<(IRClonePath, IRClonePath)> operations);
    }
}