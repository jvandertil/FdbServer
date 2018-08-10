namespace FdbServer.Download.Caching
{
    using System.IO;
    using System.Threading.Tasks;

    internal class CachingFdbServerDownloaderDecorator : IFdbServerDownloader
    {
        private readonly IFdbServerDownloader _decorated;

        public CachingFdbServerDownloaderDecorator(IFdbServerDownloader decorated)
        {
            _decorated = decorated;
        }

        public async Task DownloadVersion(FdbServerVersion version, string destinationFile)
        {
            if (!CacheContainsVersion(version))
            {
                string destination = GetCacheFileName(version);

                using (File.Open(destination, FileMode.Create))
                {
                }

                await _decorated.DownloadVersion(version, destination);
            }

            CopyCachedVersion(version, destinationFile);
        }

        private void CopyCachedVersion(FdbServerVersion version, string destinationFile)
        {
            string source = GetCacheFileName(version);

            File.Copy(source, destinationFile, true);
        }

        private string GetCacheFileName(FdbServerVersion version)
        {
            var url = FdbServerDownloadRepository.GetUrl(version);

            string fileName = Path.Combine(CachingHelper.GetCacheFolder(), Path.GetFileName(url.Uri.LocalPath));

            return fileName;
        }

        private bool CacheContainsVersion(FdbServerVersion version)
        {
            return File.Exists(GetCacheFileName(version));
        }
    }
}
