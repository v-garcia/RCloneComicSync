namespace Lib.RClone.RCloneOperations
{
    public interface IRClonePath
    {
        string Path { get; set; }
        string RemoteName { get; set; }

        string ToString();
    }
}