using System;

namespace FdbServer.Builder
{
    internal class FdbServerUrl
    {
        public FdbServerVersion Version { get; }

        public Uri Uri { get; }

        public string Sha512Hash { get; }

        public FdbServerUrl(FdbServerVersion version, Uri uri, string sha512Hash)
        {
            Version = version;
            Uri = uri;
            Sha512Hash = sha512Hash;
        }
    }
}
