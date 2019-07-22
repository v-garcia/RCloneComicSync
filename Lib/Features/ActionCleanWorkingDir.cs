using Lib.AppConfig;
using Lib.ComicFiles;
using Lib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Features
{
    public class ActionCleanWorkingDir : IActionCleanWorkingDir
    {
        private readonly ILogWriter _logWriter;

        private readonly IAppConfig _appConfig;

        private readonly IAppConstants _constants;

        private readonly IComicFileManager _comicFileMgr;

        public ActionCleanWorkingDir(ILogWriter logWriter, IAppConfig appConfig, IAppConstants constants, IComicFileManager comicFileMgr)
        {
            _logWriter = logWriter;
            _appConfig = appConfig;
            _constants = constants;
            _comicFileMgr = comicFileMgr;
        }

        public void Run()
        {
            _comicFileMgr.ScanForDeletedFiles();
        }
    }
}
