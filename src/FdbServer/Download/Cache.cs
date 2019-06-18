namespace FdbServer.Download
{
    using System.IO;
    using System.Threading.Tasks;
    using global::FdbServer.Infrastructure;

    internal static class Cache
    {
        public static IResult CheckAvailability(FdbServerUrl url)
        {
            var fileName = GetCacheFileName(url);

            if (!File.Exists(fileName))
            {
                return new FailureResult("Cache does not contain version " + url.Version);
            }

            return IntegrityVerifier.Verify(url, fileName);
        }

        public static Task<IResult> Load(FdbServerUrl url) 
            => FdbServerDownloader.Download(url, GetCacheFileName(url));

        internal static IResult Retrieve(FdbServerUrl url, string outputFile)
        {
            return CheckAvailability(url)
                    .Then(_ => Try.Wrap(() => File.Copy(GetCacheFileName(url), outputFile, true)));
        }

        private static string GetCacheFileName(FdbServerUrl url)
        {
            string fileName = Path.Combine(GetCacheFolder(), Path.GetFileName(url.Uri.LocalPath));

            return fileName;
        }

        private static string GetCacheFolder()
        {
            var cacheFolder = Path.Combine(Path.GetTempPath(), "fdbserver-cache");

            Directory.CreateDirectory(cacheFolder);

            return cacheFolder;
        }
    }
}
