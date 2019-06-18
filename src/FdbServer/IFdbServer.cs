namespace FdbServer
{
    public interface IFdbServer
    {
        /// <summary>
        /// Gets the path to the cluster file.
        /// </summary>
        string ClusterFile { get; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        IRunningFdbServer Start();

        /// <summary>
        /// Stops the server.
        /// </summary>
        IStoppedFdbServer Stop();
    }
}
