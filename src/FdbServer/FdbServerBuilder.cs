namespace FdbServer
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public class FdbServerBuilder
    {
        private FdbServerUrl _url;
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

            _homeDirectory = Path.Combine(tempFolder, "fdbserver-" + randomSuffix);
            _logDirectory = Path.Combine(_homeDirectory, "logs");
            _dataDirectory = Path.Combine(_homeDirectory, "data");
            _clusterFile = Path.Combine(_homeDirectory, "fdb.cluster");

            _url = FdbServerDownloadRepository.GetUrl(FdbServerVersion.v5_2_5);
        }

        public FdbServerBuilder WithVersion(FdbServerVersion version)
        {
            _url = FdbServerDownloadRepository.GetUrl(version);

            return this;
        }

        public async Task<IFdbServer> BuildAsync()
        {
            // Download archive to zip
            var destinationFileName = Path.GetTempFileName();

            try
            {
#if NET46
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#endif

                using (var webclient = new HttpClient())
                {
                    var serverDownloadTask = webclient.GetStreamAsync(_url.Uri);

                    using (var destination = File.OpenWrite(destinationFileName))
                    {
                        var stream = await serverDownloadTask;

                        await stream.CopyToAsync(destination);

                        await destination.FlushAsync();
                    }
                }

                // Check hash.
                using (var hashAlgorithm = SHA512.Create())
                {
                    using (var destination = File.OpenRead(destinationFileName))
                    {
                        var hash = hashAlgorithm.ComputeHash(destination);

                        var hashString = BitConverter.ToString(hash).Replace("-", "");

                        if (!hashString.Equals(_url.Sha512Hash))
                        {
                            // TODO: What is useful here?
                            throw new Exception();
                        }
                    }
                }

                // Extract archive to home
                Directory.CreateDirectory(_homeDirectory);
                ZipFile.ExtractToDirectory(destinationFileName, _homeDirectory);
            }
            finally
            {
                File.Delete(destinationFileName);
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
