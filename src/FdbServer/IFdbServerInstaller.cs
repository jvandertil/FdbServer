namespace FdbServer
{
    using System.Threading.Tasks;
    using global::FdbServer.Infrastructure;

    internal interface IFdbServerInstaller
    {
        Task<IResult> InstallToDestination(FdbServerVersion version, string destinationFolder);
    }
}
