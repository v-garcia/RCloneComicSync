using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone.Hooks
{
    public abstract class Hook<T> : IHook where T : class
    {


        protected readonly List<ITask> _runningHooks = new List<ITask>();
        public IEnumerable<ITask> RunningHooks => _runningHooks;

        public Hook()
        {
        }

        protected ITask RunTask(T arg)
        {
            var task = Handler(arg);
            _runningHooks.Add(task);
            return task.AsTask().ContinueWith(t1 =>
            {
                _runningHooks.Remove(task);
            }).AsITask();
        }

        protected abstract ITask Handler(T arg);

        public abstract void Enable();

        public abstract void Disable();
    }
}
