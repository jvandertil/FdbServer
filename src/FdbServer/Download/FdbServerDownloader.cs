namespace FdbServer.Download
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using global::FdbServer.Infrastructure;

    internal static class FdbServerDownloader
    {
        public static Task<IResult> Download(FdbServerUrl url, string destinationFile)
            => Try.Wrap(async () =>
            {
                using (var webclient = new HttpClient())
                {
                    var serverDownloadTask = webclient.GetStreamAsync(url.Uri).ConfigureAwait(false);

                    using (var destination = File.OpenWrite(destinationFile))
                    {
                        var stream = await serverDownloadTask;

                        await stream.CopyToAsync(destination).ConfigureAwait(false);

                        await destination.FlushAsync().ConfigureAwait(false);
                    }
                }
            });
    }
}
