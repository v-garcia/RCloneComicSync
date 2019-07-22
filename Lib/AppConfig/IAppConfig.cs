using Lib.RClone.RCloneOperations;
using System;
using System.Collections.Generic;

namespace Lib.AppConfig
{
    public interface IAppConfig
    {
        string WorkingFolder { get; set; }
        string DestinationFolder { get; set; }
        string FilesToSync { get; set; }
        string RCloneLogPath { get; set; }
        Nullable<int> StartCopyEveryMinuts { get; set; }
        List<IRClonePath> SourceFolders { get; set; }
        string RCloneExePath { get; set; }
        string _7ZipExePath { get; set; }
        string PDFInfoExePath { get; set; }
         bool ExcludeDeletedFromCopy { get; set; }
    }
}