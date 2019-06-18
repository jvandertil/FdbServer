using System.Threading.Tasks;
using FdbServer.Infrastructure;

namespace FdbServer.Builder
{
    internal interface IFdbServerInstaller
    {
        Task<IResult> InstallToDestination(FdbServerVersion version, string destinationFolder);
    }
}
