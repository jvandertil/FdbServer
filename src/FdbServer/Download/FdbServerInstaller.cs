namespace FdbServer.Download
{
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;
    using global::FdbServer.Infrastructure;

    internal class FdbServerInstaller : IFdbServerInstaller
    {
        public async Task<IResult> InstallToDestination(FdbServerVersion version, string destinationFolder)
        {
            var zipFileName = Path.GetTempFileName();

            var url = FdbServerUrlRepository.GetUrl(version);

            var result = (await Cache
                .CheckAvailability(url)
                    .OnFailureAsync(_ => Cache.Load(url)).ConfigureAwait(false))
                .Then(_ => Cache.Retrieve(url, zipFileName))
                .Then(_ => ExtractArchive(zipFileName, destinationFolder))
                .ThenAlways(_ => File.Delete(zipFileName));

            return result;
        }

        private static IResult ExtractArchive(string fileName, string destinationFolder)
            => Try.Wrap(() => ZipFile.ExtractToDirectory(fileName, destinationFolder));
    }
}
