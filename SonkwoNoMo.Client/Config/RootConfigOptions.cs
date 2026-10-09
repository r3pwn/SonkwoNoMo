namespace SonkwoNoMo.Client.Config;

internal class RootConfigOptions
{
    public LoggingOptions Logging { get; set; } = new();
    public GatewayOptions Gateway { get; set; } = new();
}