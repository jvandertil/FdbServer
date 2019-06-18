namespace FdbServer
{
    public interface IRunningFdbServer
    {
        /// <summary>
        /// Initializes the server, required on newly created servers.
        /// </summary>
        void Initialize();
    }
}
