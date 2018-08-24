# FdbServer
Library that provides a FoundationDB server for integration testing.

## Quick Start
Once installed (package is available through [NuGet](https://www.nuget.org/packages/FdbServer)) create a server with all defaults.

```cs
var builder = new FdbServerBuilder();
var server = await builder.BuildAsync();

try
{
    // Start the server
    server.Start();

    // Initialize the database as empty.
    server.Initialize();

    // The server is now running and available.
    // The ClusterFile property on the server can be used to connect.
}
finally
{
    // Stop the server
    server.Stop();

    // Destroy all data stored by the server.
    server.Destroy();
}
```

### Example xUnit fixture

```cs
using System;
using FdbServer;

public sealed class FdbFixture : IAsyncLifetime
{
    private IFdbServer _server;
    private bool _disposed;

    public string ClusterFile => _server.ClusterFile;

    public FdbFixture()
    {
        _disposed = false;
    }

    public async Task InitializeAsync()
    {
        var builder = new FdbServerBuilder()
            .WithVersion(FdbServerVersion.v5_2_5);

        _server = await builder.BuildAsync();

        _server.Start();
        _server.Initialize();
    }

    public Task DisposeAsync()
    {
        _server.Stop();
        _server.Destroy();

        return Task.CompletedTask;
    }
}
```

## How does it work?
The FdbServerBuilder class will connect to GitHub and download a FoundationDB server package (zip file) with the version specified.
This package will be extracted to the user's temp directory, all the data and logs are also stored in the same temporary folder.
Calling the Destroy method on the server instance will delete the folder and all files in it.