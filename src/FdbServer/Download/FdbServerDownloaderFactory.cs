namespace FdbServer.Download
{
    using Caching;
    using Integrity;

    internal class FdbServerDownloaderFactory
    {
        public static IFdbServerDownloader Create()
        {
            return new VerifyingFdbServerDownloaderDecorator(
                new CachingFdbServerDownloaderDecorator(
                    new FdbServerDownloader()));
        }
    }
}
