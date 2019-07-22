using Lib.RClone.RCloneOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.AppConfig
{
    [Serializable]
    public class AppConfig : IAppConfig
    {
        public string WorkingFolder {get; set;}
        public string DestinationFolder {get; set;}
        public string FilesToSync {get; set;}
        public string RCloneLogPath {get; set;}
        public int? StartCopyEveryMinuts {get; set;}
        public List<IRClonePath> SourceFolders {get; set;}
        public string RCloneExePath {get; set;}
        public string _7ZipExePath {get; set;}
        public string PDFInfoExePath {get; set;}
        public bool ExcludeDeletedFromCopy { get; set; }
    }

    [Serializable]
    public class SourceFolder: IRClonePath
    {
        public String RemoteName { get; set; }

        public String Path { get; set; }

        public override string ToString()
        {
            return $"{RemoteName}:{Path}";
        }
    }
    
}
