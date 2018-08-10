namespace FdbServer
{
    using System;

    internal class FdbServerUrl
    {
        public Uri Uri { get; }

        public string Sha512Hash { get; }

        public FdbServerUrl(Uri uri, string sha512Hash)
        {
            Uri = uri;
            Sha512Hash = sha512Hash;
        }
    }
}
