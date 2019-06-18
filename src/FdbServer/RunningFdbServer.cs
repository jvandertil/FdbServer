using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FdbServer
{
    internal class RunningFdbServer : FdbServerBase, IRunningFdbServer
    {
        private readonly string _fdbcliExe;

        public RunningFdbServer(FdbServerBase server)
            : base(server)
        {
            _fdbcliExe = Path.Combine(HomeDirectory, "fdbcli.exe");
        }

        public void Initialize()
        {
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
    }
}
