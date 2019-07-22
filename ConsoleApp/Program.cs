using Autofac;
using Autofac.Core;
using Lib.AppConfig;
using Lib.ComicFiles;
using Lib.ComicFilesChecker.Checkers;
using Lib.ComicFilesChecker.ComicFileVerificationManager;
using Lib.Features;
using Lib.Logger;
using Lib.RClone;
using Lib.RClone.Hooks;
using Lib.RClone.RCloneOperations;
using Lib.RClone.Watchers;
using Lib.Tools.HardLinks;
using PowerArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        public static IContainer MainContainer;
        static private IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            #region appConfig
            builder.RegisterType<AppConstants>().As<IAppConstants>();

            builder.RegisterType<ConfigHolder>().SingleInstance();

            builder.RegisterType<AppConfigManager>().As<IAppConfigManager>().SingleInstance();

            builder.Register(c => c.Resolve<ConfigHolder>().GetAppConfig()).As<IAppConfig>();
            #endregion

            #region consoleApp
            builder.RegisterType<ConsoleLogWriter>().As<ILogWriter>().SingleInstance();
            #endregion

            #region rcloneMgmt
            builder.RegisterType<RClone>()
                .As<IRClone>()
                .WithParameter(new ResolvedParameter(
                    (p, ctx) => p.ParameterType == typeof(string) && p.Name == "rcloneExePath",
                    (p, ctx) => ctx.Resolve<IAppConfig>().RCloneExePath
                ));

            builder.RegisterType<RCLoneOperationsManagerAutoRefreshed>()
                .As<IRCLoneOperationsManagerAutoRefreshed>()
                .As<IRCloneOperationsManager>()
                .WithParameter("secondsBetweenRefresh", 3)
                .SingleInstance();

            builder.RegisterType<RCloneApi>()
                .As<IRCloneApi>();

            builder.RegisterType<RCloneLogWatcher>()
                .As<IRCloneLogWatcher>()
                .As<IRCloneLogWatcher>()
                .SingleInstance()
                .WithParameter(new ResolvedParameter(
                    (p, ctx) => p.ParameterType == typeof(string) && p.Name == "filePath",
                    (p, ctx) => ctx.Resolve<IAppConfig>().RCloneLogPath
                ));
            #endregion

            #region featuresFacade
            builder.RegisterType<ActionCopy>().As<IActionCopy>();

            builder.RegisterType<ActionCleanWorkingDir>().As<IActionCleanWorkingDir>();
            #endregion

            #region fileChecks
            builder.RegisterType<PDFInfo>()
                .As<IPDFInfo>()
                .WithParameter(new ResolvedParameter(
                    (p, ctx) => p.ParameterType == typeof(string) && p.Name == "exePath",
                    (p, ctx) => ctx.Resolve<IAppConfig>().PDFInfoExePath
                ));

            builder.RegisterType<_7Zip>()
                   .As<I_7Zip>()
                   .WithParameter(new ResolvedParameter(
                        (p, ctx) => p.ParameterType == typeof(string) && p.Name == "exePath",
                        (p, ctx) => ctx.Resolve<IAppConfig>()._7ZipExePath
                    ));

            builder.RegisterType<ComicFileCheck>()
                   .As<IComicFileCheck>()
                   .SingleInstance()
                   .WithParameter(new ResolvedParameter(
                        (p, ctx) => p.ParameterType == typeof(string) && p.Name == "workingDirectory",
                        (p, ctx) => ctx.Resolve<IAppConfig>().WorkingFolder)
                   )
                   .WithParameter(new ResolvedParameter(
                        (p, ctx) => p.ParameterType == typeof(IEnumerable<IFileChecker>) && p.Name == "checkers",
                        (p, ctx) => new List<IFileChecker>() { ctx.Resolve<IPDFInfo>(), ctx.Resolve<I_7Zip>() }
                    ));

            builder.RegisterType<ComicFileManager>()
                   .As<IComicFileManager>()
                   .WithParameter(new ResolvedParameter(
                        (p, ctx) => p.ParameterType == typeof(string) && p.Name == "workingDirectory",
                        (p, ctx) => ctx.Resolve<IAppConfig>().WorkingFolder
                    ))
                   .WithParameter(new ResolvedParameter(
                        (p, ctx) => p.ParameterType == typeof(string) && p.Name == "destinationDirectory",
                        (p, ctx) => ctx.Resolve<IAppConfig>().DestinationFolder
                    ));

            #endregion

            #region hooks
            builder.RegisterType<CheckCopiedFileHook>().As<ICheckCopiedFileHook>();
            #endregion

            #region tools
            builder.RegisterType<HardLinks>().As<IHardLinks>();
            #endregion

            return builder.Build();
        }

        static void Main(string[] args)
        {
            MainContainer = CompositionRoot();
            Args.InvokeAction<AppArguments>(args);
        }
    }
}