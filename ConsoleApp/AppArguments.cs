using Autofac;
using Lib.AppConfig;
using Lib.Features;
using Lib.RClone;
using Lib.RClone.RCloneOperations;
using Nito.AsyncEx;
using PowerArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [ArgDescription("A program for syncing and checking comics files for your windows device.")]
    [ArgExample("comicsync copy remote1:path;remote2:path --DestinationDir C://Dest","Copy and checks the comics from several rclone remote to a local directory")]
    public class AppArguments
    {

        [ArgActionMethod]
        [ArgDescription("Copy the comic files")]
        public async void Copy(CopyComicArgs args) {
            using (var scope = Program.MainContainer.BeginLifetimeScope())
            {
                scope.Resolve<ConfigHolder>().MergeWithDefaultCheckAndSetAppConfig(args);

                Action copy = () => scope.Resolve<IActionCopy>().Run(() => Environment.Exit(0));
    
                AsyncContext.Run(copy);

                Console.ReadKey();
            }
        }

        [ArgActionMethod]
        [ArgDescription("Start RClone config to manage cloud storage services")]
        public void ConfigRemotes(RCloneConfigArgs args)
        {
            using (var scope = Program.MainContainer.BeginLifetimeScope())
            {
                scope.Resolve<ConfigHolder>().MergeWithDefaultCheckAndSetAppConfig(new AppConfig()
                {
                    RCloneExePath = args.RCloneExePath
                });
                scope.Resolve<IRClone>().ShowRCloneConfig();
            }
        }

        [ArgActionMethod]
        [ArgDescription("Looks for deleted folder in destination folder and remove associated cache in the working directory")]
        public void Clean(CleanWorkingFolderArgs args)
        {
            using (var scope = Program.MainContainer.BeginLifetimeScope())
            {
                scope.Resolve<ConfigHolder>().MergeWithDefaultCheckAndSetAppConfig(new AppConfig()
                {
                    WorkingFolder = args.WorkingFolder
                });
                scope.Resolve<IActionCleanWorkingDir>().Run();

                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
            }

        }

        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

    }

    public class CleanWorkingFolderArgs
    {
        [ArgDescription("The place where are stored the files before they are checked")]
        [ArgExistingDirectory]
        public string WorkingFolder { get; set; }
    }

    public class RCloneConfigArgs
    {
        [ArgDescription("Path to rclone exe path")]
        [ArgExistingFile]
        public String RCloneExePath { get; set; }
    }

    public class CopyComicArgs: IAppConfig
    {
        [ArgRequired]
        [ArgPosition(1)]
        [ArgDescription("The list of remotes path to sync separated with spaces")]
        public List<IRClonePath> SourceFolders { get; set; }

        [ArgReviver]
        public static IRClonePath Revive(string key, string val)
        {
            var splitted = val.Split(':');
            if(splitted.Length != 2)
            {
                throw new ArgException("source must be in the following form: 'remoteName:path'");
            }

            return new SourceFolder() { RemoteName = splitted[0], Path = splitted[1] };
        }

        [ArgDescription("The place where the checked files land on")]
        [ArgExistingDirectory]
        public string DestinationFolder { get; set; }

        [ArgDescription("If set copy keep copying every X minuts")]
        public Nullable<int> StartCopyEveryMinuts { get; set; }

        [ArgDescription("Choose the files to sync (rclone include)")]
        public string FilesToSync { get; set; }

        [ArgDescription("Path to 7Zip exe path")]
        [ArgExistingFile]
        public String _7ZipExePath { get; set; }

        [ArgDescription("Path to PDFInfo exe path")]
        [ArgExistingFile]
        public String PDFInfoExePath { get; set; }

        [ArgDescription("Where you want rclone write the logs")]
        public string RCloneLogPath { get; set; }

        [ArgDescription("Path to rclone exe path")]
        [ArgExistingFile]
        public String RCloneExePath { get; set; }

        [ArgDescription("The place where are stored the files before they are checked")]
        [ArgExistingDirectory]
        public string WorkingFolder { get; set; }

        [ArgDescription("Exclude deleted file from rclone copy")]
        [ArgDefaultValue(true)]
        public bool ExcludeDeletedFromCopy { get; set; }
    }
}
