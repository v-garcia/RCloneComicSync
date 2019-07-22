using MorseCode.ITask;
using System.Collections.Generic;

namespace Lib.RClone.Hooks
{
    public interface IHook
    {
        void Disable();
        void Enable();
        IEnumerable<ITask> RunningHooks { get; }
    }
}