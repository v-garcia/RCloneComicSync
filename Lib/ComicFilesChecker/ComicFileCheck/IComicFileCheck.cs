using MorseCode.ITask;

namespace Lib.ComicFilesChecker.ComicFileVerificationManager
{
    public interface IComicFileCheck
    {
        ITask<bool> CheckFile(string relativePath);
    }
}