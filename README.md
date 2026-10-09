# SonkwoNoMo
SonkwoNoMo is a community project dedicated to bringing **Laser League: World Arena** back online after its official servers were shut down in 2023.

The project provides a replacement server and client components that allow the game to communicate with community-hosted services instead of the original infrastructure.

## Project status
This project is a work in progress. The goal is to restore as much of the original online experience as possible while keeping the project straightforward to host and maintain.

## Projects
This repository contains the following projects:

- **SonkwoNoMo.Client**: Client-side components required to connect Laser League to the replacement services.
- **SonkwoNoMo.Server.Gateway**: The replacement region advertisement service used by the game to discover regions and access services.
- **SonkwoNoMo.Server.Access**: The replacement access service the game connects to; typically hosted once per region.
- **SonkwoSniffer**: Development tooling used while investigating the original game's network behavior.

See the README in each project for project-specific information and configuration.

## Getting started

If you're looking to host your own server, start with the Server README in `SonkwoNoMo.Server.Gateway`.

If you're working with the client, see the Client README.

## Disclaimer
SonkwoNoMo is an independent community project and is not affiliated with or endorsed by Roll7, 505 Games, or any other rights holder associated with Laser League.

Laser League and its related trademarks and intellectual property belong to their respective owners.

This project does not include the original game's assets. You must obtain and use the game through legitimate means.

## License
SonkwoNoMo is licensed under the MIT License.