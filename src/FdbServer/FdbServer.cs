namespace FdbServer
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    internal class FdbServer : IFdbServer
    {
        private readonly string _homeDirectory;
        private readonly string _dataDirectory;
        private readonly string _logDirectory;
        private readonly string _fdbserverExe;
        private readonly string _fdbcliExe;

        private int _port;
        private Process _fdbserverProcess;

        public bool Started => _fdbserverProcess != null && !_fdbserverProcess.HasExited;

        public string ClusterFile { get;}

        public FdbServer(string homeDirectory, string dataDirectory, string logDirectory, string clusterFile)
        {
            _homeDirectory = homeDirectory;
            _dataDirectory = dataDirectory;
            _logDirectory = logDirectory;
            ClusterFile = clusterFile;

            _fdbserverExe = Path.Combine(_homeDirectory, "fdbserver.exe");
            _fdbcliExe = Path.Combine(_homeDirectory, "fdbcli.exe");
        }

        public void Initialize()
        {
            if (!Started)
            {
                throw new InvalidOperationException("Start the server before initializing it.");
            }

            var fdbCliInfo = new ProcessStartInfo(_fdbcliExe, $@"--cluster_file=""{ClusterFile}"" --exec=""configure new single memory""")
            {
                UseShellExecute = false,
            };

            var cliProc = Process.Start(fdbCliInfo);

            while (cliProc != null && !cliProc.HasExited)
            {
                Thread.Sleep(50);
            }
        }

        public void Start()
        {
            if (Started)
            {
                throw new InvalidOperationException("Server already started.");
            }

            CreateClusterFile();

            var parameters = $"--public_address=\"127.0.0.1:{_port}\"  --listen_address=\"public\""
                + $" --cluster_file=\"{ClusterFile}\""
                + $" --datadir=\"{_dataDirectory}\""
                + $" --logdir=\"{_logDirectory}\"";

            var info = new ProcessStartInfo(_fdbserverExe, parameters)
            {
                UseShellExecute = false
            };

            _fdbserverProcess = Process.Start(info);
        }

        public void Stop()
        {
            if (!Started)
            {
                throw new InvalidOperationException("Server not started.");
            }

            if (!_fdbserverProcess.HasExited)
            {
                _fdbserverProcess.CloseMainWindow();
                _fdbserverProcess.WaitForExit(1000);

                if (!_fdbserverProcess.HasExited)
                {
                    _fdbserverProcess.Kill();

                    while (!_fdbserverProcess.HasExited)
                    {
                        // Wait for process to die.
                        Thread.Sleep(50);
                    }

                    // Allow a small amount of time for the process to terminate, releasing all files.
                    Thread.Sleep(100);
                }
            }
        }

        public void Destroy()
        {
            if (Started)
            {
                throw new InvalidOperationException("Destroy can not be run while the server is running.");
            }

            Directory.Delete(_homeDirectory, true);
        }

        private void CreateClusterFile()
        {
            _port = FreeTcpPort();

            File.WriteAllText(ClusterFile, $"{Guid.NewGuid():N}:{Guid.NewGuid():N}@127.0.0.1:{_port}");
        }

        private static int FreeTcpPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);

            try
            {
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;

                return port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
