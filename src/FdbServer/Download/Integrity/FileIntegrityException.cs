namespace FdbServer.Download.Integrity
{
    using System;
    using System.Runtime.Serialization;

    public class FileIntegrityException : Exception
    {
        public FileIntegrityException()
            : base("Downloaded file checksum did not match stored checksum.")
        {
        }

        public FileIntegrityException(string message)
            : base(message)
        {
        }

        public FileIntegrityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FileIntegrityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
