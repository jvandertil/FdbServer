namespace FdbServer
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Download;
    using Download.Caching;
    using Download.Integrity;

    public class FdbServerBuilder
    {
        private readonly IFdbServerInstaller _installer = new FdbServerInstaller();

        private FdbServerVersion _version;
        private string _homeDirectory;
        private string _logDirectory;
        private string _dataDirectory;
        private string _clusterFile;

        public FdbServerBuilder()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (!isWindows)
            {
                throw new PlatformNotSupportedException("This library currently only supports Windows.");
            }

            var tempFolder = Path.GetTempPath();
            var randomSuffix = Guid.NewGuid().ToString().Substring(0, 6);

            WithHomeDirectory(Path.Combine(tempFolder, "fdbserver-" + randomSuffix));
        }

        private void WithHomeDirectory(string homeDirectory)
        {
            _homeDirectory = homeDirectory;
            _logDirectory = Path.Combine(_homeDirectory, "logs");
            _dataDirectory = Path.Combine(_homeDirectory, "data");
            _clusterFile = Path.Combine(_homeDirectory, "fdb.cluster");
        }

        public FdbServerBuilder WithVersion(FdbServerVersion version)
        {
            _version = version;

            return this;
        }

        public async Task<IFdbServer> BuildAsync()
        {
            // Download archive to zip
            try
            {
                await _installer.InstallToDestination(_version, _homeDirectory);
            }
            catch (FileIntegrityException)
            {
                // Maybe corrupt cache?
                CachingHelper.PurgeCache();

                try
                {
                    await _installer.InstallToDestination(_version, _homeDirectory);
                }
                catch
                {
                    Directory.Delete(_homeDirectory, true);

                    throw;
                }
            }

            // Initialize folders, log & data
            Directory.CreateDirectory(_logDirectory);
            Directory.CreateDirectory(_dataDirectory);

            var server = new FdbServer(
                _homeDirectory,
                _dataDirectory,
                _logDirectory,
                _clusterFile);

            return server;
        }
    }
}
