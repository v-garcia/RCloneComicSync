namespace Lib.RClone.RCloneOperations
{
    public interface IRCloneCopyOperation
    {
        IRClonePath destination { get; set; }
        int jobId { get; set; }
        IRClonePath source { get; set; }

        string ToString();
    }
}