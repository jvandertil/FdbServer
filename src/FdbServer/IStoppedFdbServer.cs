namespace FdbServer
{
    public interface IStoppedFdbServer : IFdbServer
    {
        /// <summary>
        /// Starts the server.
        /// </summary>
        IRunningFdbServer Start();

        /// <summary>
        /// Destroys the server, deleting all files created.
        /// </summary>
        void Destroy();
    }
}
