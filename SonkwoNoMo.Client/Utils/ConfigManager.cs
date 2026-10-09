using System.Reflection;
using Microsoft.Extensions.Configuration;
using SonkwoNoMo.Client.Config;

namespace SonkwoNoMo.Client.Utils;

public class ConfigManager
{
    private static readonly Lazy<RootConfigOptions> ConfigInstance = new(BuildConfiguration);

    public static RootConfigOptions Instance => ConfigInstance.Value;

    private static RootConfigOptions BuildConfiguration()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder();
        if (!string.IsNullOrEmpty(PathHelper.BasePath))
        {
            builder = builder.SetBasePath(PathHelper.BasePath);
        }
        var config = builder
            .AddIniFile("config.ini", optional: true, reloadOnChange: true)
            .Build();

        var userConfig = new RootConfigOptions();
        config.GetSection("Logging").Bind(userConfig.Logging);

        return userConfig;
    }
}
