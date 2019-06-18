using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FdbServer
{
    internal class RunningFdbServer : FdbServerBase, IRunningFdbServer
    {
        private readonly string _fdbcliExe;
        private readonly Process _serverProcess;

        private bool _disposed;

        public RunningFdbServer(FdbServerBase server, Process serverProcess)
            : base(server)
        {
            _fdbcliExe = Path.Combine(HomeDirectory, "fdbcli.exe");
            _serverProcess = serverProcess;

            _disposed = false;
        }

        public IRunningFdbServer Initialize()
        {
            var fdbCliInfo = new ProcessStartInfo(_fdbcliExe, $@"--cluster_file=""{ClusterFile}"" --exec=""configure new single memory""")
            {
                UseShellExecute = false,
            };

            using (var cliProc = Process.Start(fdbCliInfo))
            {
                while (cliProc != null && !cliProc.HasExited)
                {
                    Thread.Sleep(50);
                }

                return this;
            }
        }

        public IStoppedFdbServer Stop()
        {
            Dispose();

            return new StoppedFdbServer(this);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                if (!_serverProcess.HasExited)
                {
                    _serverProcess.CloseMainWindow();
                    _serverProcess.WaitForExit(1000);

                    if (!_serverProcess.HasExited)
                    {
                        _serverProcess.Kill();

                        while (!_serverProcess.HasExited)
                        {
                            // Wait for process to die.
                            Thread.Sleep(50);
                        }

                        // Allow a small amount of time for the process to terminate, releasing all files.
                        Thread.Sleep(100);
                    }
                }
            }
            finally
            {
                _serverProcess.Dispose();
            }
        }
    }
}
