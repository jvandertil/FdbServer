namespace FdbServer
{
    public interface IFdbServer
    {
        /// <summary>
        /// Gets the path to the cluster file.
        /// </summary>
        string ClusterFile { get; }

        /// <summary>
        /// Kills and destroys this server.
        /// </summary>
        /// <seealso cref="IRunningFdbServer.Stop"/>
        /// <seealso cref="IStoppedFdbServer.Destroy"/>
        void Destruct();
    }
}
