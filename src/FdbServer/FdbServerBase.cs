using System;

namespace FdbServer
{
    internal abstract class FdbServerBase
    {
        protected readonly string HomeDirectory;
        protected readonly string DataDirectory;
        protected readonly string LogDirectory;

        public string ClusterFile { get; }

        protected FdbServerBase(string homeDirectory, string dataDirectory, string logDirectory, string clusterFile)
        {
            HomeDirectory = homeDirectory;
            DataDirectory = dataDirectory;
            LogDirectory = logDirectory;
            ClusterFile = clusterFile;
        }

        protected FdbServerBase(FdbServerBase server)
        {
            if (server is null)
                throw new ArgumentNullException(nameof(server));

            HomeDirectory = server.HomeDirectory;
            DataDirectory = server.DataDirectory;
            LogDirectory = server.LogDirectory;
            ClusterFile = server.ClusterFile;
        }
    }
}
