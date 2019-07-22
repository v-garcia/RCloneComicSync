using MorseCode.ITask;
using System;

namespace Lib.Features
{
    public interface IActionCopy
    {
        void Run(Action onFinish);
    }
}