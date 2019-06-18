using System;

namespace FdbServer
{
    public interface IRunningFdbServer : IDisposable
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
