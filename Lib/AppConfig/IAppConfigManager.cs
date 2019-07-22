using System.Collections.Generic;

namespace Lib.AppConfig
{
    public interface IAppConfigManager
    {
        IAppConfig GetDefaultConfig();
        (bool, IEnumerable<string>) IsAppConfigValid(IAppConfig appConfig);
        IAppConfig MergeWithDefault(IAppConfig appConfig);
        void CreateDirectoriesAndFiles(IAppConfig appConfig);
    }
}