using SonkwoNoMo.Client.Config;

namespace SonkwoNoMo.Client.Logging;

internal class LoggerManager
{
    private static readonly Lazy<ILogger> ConfigInstance = new(BuildLogger);

    public static ILogger Instance => ConfigInstance.Value;

    private static ILogger BuildLogger()
    {
        var loggerType = ConfigLoader.Instance.Logging.Output.ToLower();

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