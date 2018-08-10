namespace FdbServer.Download
{
    using System.Threading.Tasks;

    internal interface IFdbServerDownloader
    {
        Task DownloadVersion(FdbServerVersion version, string destinationFile);
    }
}
