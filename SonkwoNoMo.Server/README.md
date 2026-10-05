# SonkwoNoMo Server
The SonkwoNoMo Server provides the replacement online services used by the **SonkwoNoMo Client** to bring Laser League: World Arena back online.

The server consists of two services:

- **Region advertisement**: An HTTP service that provides the client with information about available regions and where their access services can be found.
- **Access service**: The service the client connects to after receiving the region information.

## Requirements
- .NET 10 SDK
- A machine capable of accepting connections from the game client

## Configuration
Server configuration is stored in `appsettings.json`.

## Running
To run the server directly from the repository:

```
dotnet run --project SonkwoNoMo.Server
```

By default:

- The **region advertisement service** listens on port `5118`.
- The **access service** listens on port `4444`.

## Building
Build the server with:

```
dotnet build -c Release
```

## Hosting
The server can be hosted on a machine accessible to the players who will be using it.

When hosting remotely, make sure:

1. The region advertisement service is accessible to the client.
2. The addresses provided by the region advertisement point to publicly reachable access services.
3. The relevant ports are allowed through the server's firewall.
4. The configured addresses match the actual hostnames or IP addresses used by the deployment.

The client uses the region information to determine where to connect to the access service, so the access service is not required to use its default port of `4444`.

## Status
The server is still under development. It currently implements the portions of the original online services that have been reconstructed, with additional functionality being added as the project progresses.

For client-side setup and configuration, see the Client README.
