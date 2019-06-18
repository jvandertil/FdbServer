using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FdbServer
{
    internal class FdbServerInstance : FdbServerBase, IFdbServer
    {
        private readonly string _fdbserverExe;

        private int _port;
        private Process _fdbserverProcess;

        public bool Started => _fdbserverProcess != null && !_fdbserverProcess.HasExited;

        public FdbServerInstance(string homeDirectory, string dataDirectory, string logDirectory, string clusterFile)
            : base(homeDirectory, dataDirectory, logDirectory, clusterFile)
        {
            _fdbserverExe = Path.Combine(HomeDirectory, "fdbserver.exe");
        }

        public IRunningFdbServer Start()
        {
            if (Started)
            {
                throw new InvalidOperationException("Server already started.");
            }

            CreateClusterFile();

            var parameters = $"--public_address=\"127.0.0.1:{_port}\"  --listen_address=\"public\""
                + $" --cluster_file=\"{ClusterFile}\""
                + $" --datadir=\"{DataDirectory}\""
                + $" --logdir=\"{LogDirectory}\"";

            var info = new ProcessStartInfo(_fdbserverExe, parameters)
            {
                UseShellExecute = false
            };

            _fdbserverProcess = Process.Start(info);

            return new RunningFdbServer(this);
        }

        public IStoppedFdbServer Stop()
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

            return new StoppedFdbServer(this);
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
