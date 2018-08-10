namespace FdbServer
{
    using System.Threading.Tasks;

    internal interface IFdbServerInstaller
    {
        Task InstallToDestination(FdbServerVersion version, string destinationFolder);
    }
}
