namespace Lib.ComicFiles
{
    public interface IComicFileManager
    {
        void LinkFileToDestinationDir(string relativePath);
        void ScanForDeletedFiles();
    }
}