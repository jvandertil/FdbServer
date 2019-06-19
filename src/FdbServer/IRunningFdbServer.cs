using System;

namespace FdbServer
{
    public interface IRunningFdbServer : IFdbServer, IDisposable
    {
        /// <summary>
        /// Initializes the server, required on newly created servers.
        /// </summary>
        IRunningFdbServer Initialize();

        /// <summary>
        /// Stops the server.
        /// </summary>
        IStoppedFdbServer Stop();
    }
}
