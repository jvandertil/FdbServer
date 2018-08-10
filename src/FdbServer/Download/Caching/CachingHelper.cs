namespace FdbServer.Download.Caching
{
    using System.IO;

    internal static class CachingHelper
    {
        internal static void PurgeCache()
        {
            Directory.Delete(GetCacheFolder(), true);
        }

        internal static string GetCacheFolder()
        {
            var cacheFolder = Path.Combine(Path.GetTempPath(), "fdbserver-cache");

            Directory.CreateDirectory(cacheFolder);

            return cacheFolder;
        }
    }
}
