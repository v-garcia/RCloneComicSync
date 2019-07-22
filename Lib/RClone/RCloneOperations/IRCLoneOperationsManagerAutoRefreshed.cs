namespace Lib.RClone.RCloneOperations
{
    public interface IRCLoneOperationsManagerAutoRefreshed: IRCloneOperationsManager
    {
        void StartRefresh();
        void StopRefresh();
    }
}