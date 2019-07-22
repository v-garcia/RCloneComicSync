namespace Lib.Tools.HardLinks
{
    public interface IHardLinks
    {
        void CreateHardLink(string from, string to);
        int GetOtherHardLinksCount(string filepath);
    }
}