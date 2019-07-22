using MorseCode.ITask;

namespace Lib.ComicFilesChecker.Checkers
{
    public interface IFileChecker
    {
        string Name { get; }

        bool CanCheckFile(string fileName);
        ITask<bool> CheckFilePath(string fileName);
    }
}