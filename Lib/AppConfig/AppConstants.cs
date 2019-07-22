using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.AppConfig
{
    public class AppConstants : IAppConstants
    {
        public string LinkedFilesFileName { get; } = "linked_files";

        public string DeletedFilesFileName { get; } = "deleted_files";

        public string RCloneLocalRemoteName { get; } = "local";
    }
}
