# SonkwoNoMo Client
The SonkwoNoMo Client provides the client-side components required to connect **Laser League: World Arena** to a SonkwoNoMo server.

The project is built for .NET 10 and is published as a native shared library using .NET Native AOT (Ahead-of-Time) deployments.

## Configuration
The client uses `config.ini` for configuration.

A default configuration is included as `default.config.ini`:

```
[Logging]
# other options are "console" and "file"
Output=none
```

When setting up the client, copy or rename the default configuration to `config.ini` and change values as appropriate for your installation.

### Logging
Logging is controlled by the `Output` setting under the `[Logging]` section.

| Value | Description |
| --- | --- |
| `none` | Disable logging |
| `console` | Log all output to a new console window |
| `file` | Write log output to a file |

For example, to enable console logging:

```
[Logging]
Output=console
```

The default configuration uses `none`.

> Additional configuration options may be added as client development continues.

## Building
Building the client requires the **.NET 10 SDK**.

Publish the project in Release configuration (a `publish` ensures we're building using .NET AOT, which is required) with:

```
dotnet publish -c Release
```

The resulting native library can then be copied into your Laser League installation as a drop-in replacement for `Engine\Binaries\ThirdParty\Sonkwo\client\x64\Release\MMClientSDK.dll`.

## Development
The client is still under active development. Configuration and behavior may change as additional parts of the original game's online functionality are restored.

For information about hosting the replacement service, see the Server README.
