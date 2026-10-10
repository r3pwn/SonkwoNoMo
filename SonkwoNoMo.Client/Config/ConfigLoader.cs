using Microsoft.Extensions.Configuration;
using SonkwoNoMo.Client.Core;

namespace SonkwoNoMo.Client.Config;

internal class ConfigLoader
{
    private static readonly Lazy<RootConfigOptions> ConfigInstance = new(BuildConfiguration);

    public static RootConfigOptions Instance => ConfigInstance.Value;

    private static RootConfigOptions BuildConfiguration()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder();
        if (!string.IsNullOrEmpty(Paths.BasePath))
        {
            builder = builder.SetBasePath(Paths.BasePath);
        }
        var config = builder
            .AddIniFile("config.ini", optional: true, reloadOnChange: false)
            .Build();

        var userConfig = new RootConfigOptions();
        config.GetSection("Logging").Bind(userConfig.Logging);
        config.GetSection("Gateway").Bind(userConfig.Gateway);

        return userConfig;
    }
}
