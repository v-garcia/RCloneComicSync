using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.ComicFiles;
using Lib.ComicFilesChecker.Checkers;
using Lib.ComicFilesChecker.ComicFileVerificationManager;
using Lib.Logger;
using Lib.RClone.Watchers;
using MorseCode.ITask;

namespace Lib.RClone.Hooks
{
    public class CheckCopiedFileHook : NewFileCopiedHook, ICheckCopiedFileHook
    {
        protected readonly IComicFileCheck _checker;

        protected readonly IComicFileManager _fileMgr;


        public CheckCopiedFileHook(IComicFileCheck checker, IComicFileManager fileMgr, IRCloneLogWatcher logWatcher, ILogWriter logWriter) : base(logWatcher, logWriter)
        {
            _checker = checker;
            _fileMgr = fileMgr;
        }

        protected override async ITask Handler(IRCloneFileCopied arg)
        {
            var isFileOk = await _checker.CheckFile(relativePath: arg.FileName);

            if(isFileOk)
            {
                _fileMgr.LinkFileToDestinationDir(relativePath: arg.FileName);
            }
        }
    }
}
