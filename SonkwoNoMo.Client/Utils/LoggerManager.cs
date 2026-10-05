using Microsoft.Extensions.Configuration;
using SonkwoNoMo.Client.Config;
using SonkwoNoMo.Client.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SonkwoNoMo.Client.Utils;

public class LoggerManager
{
    private static readonly Lazy<ILogger> ConfigInstance = new(BuildLogger);

    public static ILogger Instance => ConfigInstance.Value;

    private static ILogger BuildLogger()
    {
        var loggerType = ConfigManager.Instance.Logging.Output.ToLower();

        ILogger log = loggerType switch
        {
            "console" => new ConsoleLogger(),
            "file" => new FileLogger(),
            _ => new NoopLogger()
        };

        log.Start();

        return log;
    }
}