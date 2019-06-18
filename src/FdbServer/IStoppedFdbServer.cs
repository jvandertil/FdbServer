namespace FdbServer
{
    public interface IStoppedFdbServer
    {
        /// <summary>
        /// Destroys the server, deleting all files created.
        /// </summary>
        void Destroy();
    }
}
