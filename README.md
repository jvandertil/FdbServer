# FdbServer
Library that provides a FoundationDB server for integration testing.

## Quick Start
Once installed (package is available through [NuGet](https://www.nuget.org/packages/FdbServer)) create a server with all defaults.

```cs
var server = await new FdbServerBuilder()
                            .BuildAsync()
                            .ConfigureAwait(false);

try
{
    server
        // Start the server
        .Start()
        // Initialize the database as empty.
        .Initialize();

    // The server is now running and available.
    // Use server.ClusterFile to connect to the server.
}
finally
{
    // Stop the server and delete it entirely
    server.Destruct();
}
```

### Example xUnit fixture

```cs
using System;
using FdbServer;

public sealed class FdbFixture : IAsyncLifetime
{
    private IFdbServer _server;

    public string ClusterFile => _server.ClusterFile;

    public async Task InitializeAsync()
    {
        var server = await new FdbServerBuilder()
            .WithVersion(FdbServerVersion.v5_2_5)
            .BuildAsync()
            .ConfigureAwait(false);

        _server = server
            .Start()
            .Initialize();
    }

    public Task DisposeAsync()
    {
        _server.Destruct();

        return Task.CompletedTask;
    }
}
```

## How does it work?
The FdbServerBuilder class will connect to GitHub and download a FoundationDB server package (zip file) with the version specified.
This package will be extracted to the user's temp directory, all the data and logs are also stored in the same temporary folder.
Calling the Destroy method on the server instance will delete the folder and all files in it.

To reduce the amount of bandwidth consumed the zip files will be cached in the temp folder as well.

## Limitations
This is just something I've thrown together quite quickly, so there are some limitations.
Please open an issue or pull request if you need anything.

Known limitations:
* Currently only works on Windows
  * Supporting other operating systems is not on my list for now.
* Little to no configuration options.
  * I'll add options that I find useful myself, if you need to configure something please open an issue.
* Resuming a server between sessions is not supported
  * Every server gets a new directory, you can't specify a location currently.
* Only supports FoundationDB 5.2.5
  * I'll add newer versions as they appear.