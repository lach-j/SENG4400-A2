# Setup

## Prerequisites

### .NET 6.0

All three projects within the solution are built using the .NET 6.0 framework. To build and run the projects, the .NET 6 SDK and Runtime must be installed. The installation will depend on your operating system and instructions can be found [here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).

### Azure Storage Emulator

To run the `A2.ClientFunc` project locally the Azure Storage emulator needs to be installed.

For Windows this can be found [here](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator).

For Linux, the Azurite package can be installed using npm:

```
npm install -g azurite
```

### Azure Functions Core Tools

In addition to the Azure Storage Emulator, the Azure Function Core Tools will need to be installed. This can be found [here](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cwindows%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools).

# Building and Running the Solution

## Server and Dashboard

To run the server and the dashboard:

1. Open 2 terminal instances.
2. In the first instance run the command:

```
dotnet run --project .\A2.Dashboard\A2.Dashboard.csproj
```

This should restore all dependencies, build the project and run the Dashboard. You can verify this has worked by visiting https://localhost:7189.

3. In the second terminal instance, run the command:

```
dotnet run --project .\A2.Server\A2.Server.csproj
```

### Client Function

Before starting the `A2.ClientFunc` project, the Azure Storage Emulator must be running. To start this service open a terminal instance and run the command:

```
"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" start
```

**NOTE:** The command mentioned is only for the Windows Azure Storage Emulator. For starting the linux service please refer to the Azurite documentation.

Once the Storage Emulator has started, the function app is able to be started.

Navigate to the `A2.ClientFunc` directory and run the following command to start the function trigger:

```
func start
```

With all three projects running you should start to see results come through to the dashboard.

# Configuration

## Server

The following values can be configured in the `appsettings.json` file under the `"AppSettings"` section.

_MaxNumber:_ The maximum number that the server will generate (Default: 1000000)

_DelayInterval:_ The amount of time in milliseconds that the server will wait before generating the next number (Default: 1000)

_ServiceBusConnectionString:_ The connection string to the `primenums` queue on the Azure Service Bus. Requires Send/Write access. E.g., `Endpoint=sb://<service-bus-endpoint>;SharedAccessKeyName=<key-name>;SharedAccessKey=<access-key>;EntityPath=primenums`

## Client Function

Configuration for the client function can be found in the `local.settings.json` file.

_A2ServiceBus_SERVICEBUS:_ The connection string to the `primenums` queue on the Azure Service Bus. Requires Send/Write access. E.g., `Endpoint=sb://<service-bus-endpoint>;SharedAccessKeyName=<key-name>;SharedAccessKey=<access-key>;EntityPath=primenums`

_A2Dashboard_BASEURL:_ The base URL of the dashboard application e.g., `https://localhost:7189`

_A2Dashboard_USERNAME:_ Username for the dashboard endpoint. This will need to be `admin` as this is not configurable server side.

_A2Dashboard_PASSWORD:_ Password for the dashboard endpoint. This will need to be `password` as this is not configurable server side.
