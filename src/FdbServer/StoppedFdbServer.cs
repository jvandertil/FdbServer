using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FdbServer
{
    internal class StoppedFdbServer : FdbServerBase, IStoppedFdbServer
    {
        private readonly string _fdbserverExe;

        private int _port;
        private Process _serverProcess;

        public bool Started => _serverProcess != null && !_serverProcess.HasExited;

        public StoppedFdbServer(string homeDirectory, string dataDirectory, string logDirectory, string clusterFile)
            : base(homeDirectory, dataDirectory, logDirectory, clusterFile)
        {
            _fdbserverExe = Path.Combine(HomeDirectory, "fdbserver.exe");
        }

        public StoppedFdbServer(FdbServerBase server)
            : base(server)
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

            _serverProcess = Process.Start(info);

            return new RunningFdbServer(this, _serverProcess);
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

        public void Destroy()
        {
            Exception caughtException;
            do
            {
                try
                {
                    Directory.Delete(HomeDirectory, true);
                    caughtException = null;
                }
                catch (Exception e)
                {
                    caughtException = e;
                }
            }
            while (caughtException != null);
        }

        public override void Destruct()
            => Destroy();
    }
}
