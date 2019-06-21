using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FdbServer.Infrastructure;

namespace FdbServer.Builder.Download
{
    internal static class FdbServerDownloader
    {
        public static Task<IResult> Download(FdbServerUrl url, string destinationFile)
            => Try.Wrap(async () =>
            {
                using (var webclient = new HttpClient())
                {
                    var serverDownloadTask = webclient.GetStreamAsync(url.Uri);

                    using (var destination = File.OpenWrite(destinationFile))
                    {
                        var stream = await serverDownloadTask.ConfigureAwait(false);

                        await stream.CopyToAsync(destination).ConfigureAwait(false);

                        await destination.FlushAsync().ConfigureAwait(false);
                    }
                }
            });
    }
}
