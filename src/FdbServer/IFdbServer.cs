namespace FdbServer
{
    public interface IFdbServer
    {
        /// <summary>
        /// Gets the path to the cluster file.
        /// </summary>
        string ClusterFile { get; }

        /// <summary>
        /// Initializes the server, required on newly created servers.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Starts the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Destroys the server, deleting all files created.
        /// </summary>
        void Destroy();
    }
}
