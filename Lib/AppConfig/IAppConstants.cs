namespace Lib.AppConfig
{
    public interface IAppConstants
    {
        string DeletedFilesFileName { get; }
        string LinkedFilesFileName { get; }

        string RCloneLocalRemoteName { get; }
    }
}