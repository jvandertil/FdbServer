namespace FdbServer.Download
{
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal class FdbServerDownloader : IFdbServerDownloader
    {
        public async Task DownloadVersion(FdbServerVersion version, string destinationFile)
        {
            var url = FdbServerUrlRepository.GetUrl(version);

#if NET46
            // Enable TLS 1.2 for GitHub.
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
#endif

            // Download file to temporary file.
            using (var webclient = new HttpClient())
            {
                var serverDownloadTask = webclient.GetStreamAsync(url.Uri);

                using (var destination = File.OpenWrite(destinationFile))
                {
                    var stream = await serverDownloadTask;

                    await stream.CopyToAsync(destination);

                    await destination.FlushAsync();
                }
            }
        }
    }

    internal class FdbServerInstaller : IFdbServerInstaller
    {
        private readonly IFdbServerDownloader _downloader;

        public FdbServerInstaller()
        {
            _downloader = FdbServerDownloaderFactory.Create();
        }

        public async Task InstallToDestination(FdbServerVersion version, string destinationFolder)
        {
            var zipFileName = Path.GetTempFileName();

            try
            {
                await _downloader.DownloadVersion(version, zipFileName);

                // Extract archive to home
                Directory.CreateDirectory(destinationFolder);
                ZipFile.ExtractToDirectory(zipFileName, destinationFolder);
            }
            finally
            {
                File.Delete(zipFileName);
            }
        }
    }
}
