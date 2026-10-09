# SonkwoNoMo Server
The SonkwoNoMo Server provides the replacement online services used by the **SonkwoNoMo Client** to bring Laser League: World Arena back online.

The server consists of two independently deployable applications:

- **Gateway** (`SonkwoNoMo.Server.Gateway`): An HTTP service that provides the client with information about available regions and where their access services can be found.
- **Access** (`SonkwoNoMo.Server.Access`): The service the client connects to after receiving the region information. Typically hosted once per region.

## Requirements
- .NET 10 SDK
- A machine capable of accepting connections from the game client

## Configuration
Each application is configured through its own `appsettings.json`:

- The Gateway's region catalog and the access address it advertises are stored under the `RegionList` section.
- The Access service's TCP port is stored under the `Access` section (default `4444`).

## Running
To run the applications directly from the repository:

```
dotnet run --project SonkwoNoMo.Server.Gateway
dotnet run --project SonkwoNoMo.Server.Access
```

By default (when running from the repository):

- The **Gateway** listens on port `5118`.
- The **Access** service listens on TCP port `4444`.

## Building
Build the server with:

```
dotnet build -c Release
```

## Hosting
Each application can be hosted independently on a machine accessible to the players who will be using it; typically one Access instance per region.

When hosting remotely, make sure:

1. The Gateway is accessible to the client.
2. The addresses provided by the Gateway's region catalog point to publicly reachable Access services.
3. The relevant ports are allowed through the server's firewall.
4. The configured addresses and ports match the actual hostnames, IP addresses, and ports used by the deployment.

The client uses the region information to determine where to connect to the Access service, so the Access service is not required to use its default port of `4444` — set `Access:Port` and the catalog's `Location` values accordingly.

## Status
The server is still under development. It currently implements the portions of the original online services that have been reconstructed, with additional functionality being added as the project progresses.

For client-side setup and configuration, see the Client README.
