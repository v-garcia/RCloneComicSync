using Lib.AppConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class ConfigHolder
    {
        protected IAppConfig _appConfig;
        protected readonly IAppConfigManager _configManager;

        public ConfigHolder(IAppConfigManager configManager)
        {
            _configManager = configManager;
        }

        public IAppConfig GetAppConfig()
        {
            return _appConfig;
        }

        public void MergeWithDefaultCheckAndSetAppConfig(IAppConfig toSet)
        {
            // Merge with default config (see GetDefaultConfig)
            var configToSet = _configManager.MergeWithDefault(toSet);

            // Creates directories which are not created
            _configManager.CreateDirectoriesAndFiles(configToSet);

            // Check if config is valid
            var (success, errs) = _configManager.IsAppConfigValid(configToSet);

            // Exit app if config is not valid
            if(!success)
            {
                Environment.Exit(1);
            }

            //
            _appConfig = configToSet;
        }

    }
}
