using Lib.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lib.AppConfig
{
    public class AppConfigManager : IAppConfigManager
    {

        protected readonly ILogWriter _logWriter;

        protected readonly IAppConstants _constants;

        public AppConfigManager(ILogWriter logWriter, IAppConstants constants)
        {
            _logWriter = logWriter;
            _constants = constants;
        }

        public IAppConfig GetDefaultConfig()
        {
            var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var workingFolder = Path.Combine(personalFolder, "Downloads", "ComicSyncsWorkingDirectory/");
            var destinationFolder = Path.Combine(personalFolder, "Downloads", "ComicSyncs/");
            var logPath = Path.ChangeExtension(Path.GetTempFileName(), "log");
            var chocolateyBinPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "chocolatey", "bin/"
                );


            return new AppConfig()
            {
                WorkingFolder = workingFolder,
                DestinationFolder = destinationFolder,
                FilesToSync = "*.{cbz,cbr,pdf}",
                RCloneLogPath = logPath,
                RCloneExePath = Path.Combine(chocolateyBinPath, "rclone.exe"),
                PDFInfoExePath = Path.Combine(chocolateyBinPath, "pdfinfo.exe"),
                _7ZipExePath = Path.Combine(chocolateyBinPath, "7z.exe"),
                StartCopyEveryMinuts = null,
                ExcludeDeletedFromCopy = true
            };
        }

        public IAppConfig MergeWithDefault(IAppConfig appConfig)
        {
            return MergeConfigs(GetDefaultConfig(), appConfig);
        }

        private IAppConfig MergeConfigs(IAppConfig config1, IAppConfig config2)
        {
            config1 = config1 ?? new AppConfig();
            config2 = config2 ?? new AppConfig();

            var resp = new AppConfig();
            foreach (PropertyInfo propertyInfo in config1.GetType().GetProperties())
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                {
                    continue;
                }


                var firstValue = propertyInfo.GetValue(config1, null);
                var secondValue = config2.GetType().GetProperty(propertyInfo.Name).GetValue(config2,null);
                var valToSet = secondValue is null ? firstValue : secondValue;
                propertyInfo.SetValue(resp, valToSet);
            }
            return resp;
        }

        public (bool, IEnumerable<String>) IsAppConfigValid(IAppConfig appConfig)
        {
            var errorList = new List<String>();

            if (!File.Exists(appConfig.RCloneExePath))
            {
                errorList.Add($"RClone exe not found at ${appConfig.RCloneExePath}");
            }


            if (!File.Exists(appConfig.PDFInfoExePath))
            {
                errorList.Add($"PDFInfo exe not found at ${appConfig.PDFInfoExePath}");
            }


            if (!File.Exists(appConfig._7ZipExePath))
            {
                errorList.Add($"7Zip exe not found at ${appConfig._7ZipExePath}");
            }

            if (!Directory.Exists(appConfig.DestinationFolder))
            {
                errorList.Add($"Destination folder not exists: ${appConfig.DestinationFolder}");
            }

            if (!Directory.Exists(appConfig.WorkingFolder))
            {
                errorList.Add($"Working folder not exists: ${appConfig.WorkingFolder}");
            }

            if (Path.GetPathRoot(appConfig.WorkingFolder) != Path.GetPathRoot(appConfig.DestinationFolder))
            {
                errorList.Add($"Working folder and destination folder have to be on the same drive");
            }

            errorList.ForEach(x => _logWriter.WriteLine(x));

            return (!errorList.Any(), errorList);
        }

        public void CreateDirectoriesAndFiles(IAppConfig appConfig)
        {
            if(!Directory.Exists(appConfig.DestinationFolder))
            {
                _logWriter.WriteLine($"Create destinationFolder directory at '${appConfig.DestinationFolder}'");
                Directory.CreateDirectory(appConfig.DestinationFolder);
            }

            if(!Directory.Exists(appConfig.WorkingFolder))
            {
                _logWriter.WriteLine($"Create WorkingFolder directory at '${appConfig.WorkingFolder}'");
                Directory.CreateDirectory(appConfig.WorkingFolder);
            }

            var linkedFiles = Path.Combine(appConfig.WorkingFolder, _constants.LinkedFilesFileName);
            if(!File.Exists(linkedFiles))
            {
                _logWriter.WriteLine($"Create linkedFiles repo fo; at '${linkedFiles}'");
                using (File.Create(linkedFiles)) { };
            }

            var deletedFiles = Path.Combine(appConfig.WorkingFolder, _constants.DeletedFilesFileName);
            if (!File.Exists(deletedFiles))
            {
                _logWriter.WriteLine($"Create deletedFiles repo fo; at '${deletedFiles}'");
                using (File.Create(deletedFiles)) { };
            }
        }
    }
}
