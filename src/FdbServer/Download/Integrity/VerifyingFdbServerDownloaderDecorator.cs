namespace FdbServer.Download.Integrity
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    internal class VerifyingFdbServerDownloaderDecorator : IFdbServerDownloader
    {
        private readonly IFdbServerDownloader _decorated;

        public VerifyingFdbServerDownloaderDecorator(IFdbServerDownloader decorated)
        {
            _decorated = decorated;
        }

        public async Task DownloadVersion(FdbServerVersion version, string destinationFile)
        {
            await _decorated.DownloadVersion(version, destinationFile);

            var checksum = FdbServerDownloadRepository.GetUrl(version).Sha512Hash;

            using (var hashAlgorithm = SHA512.Create())
            {
                using (var destination = File.OpenRead(destinationFile))
                {
                    var hash = hashAlgorithm.ComputeHash(destination);

                    var hashString = ToHex(hash);

                    if (!hashString.Equals(checksum, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new FileIntegrityException();
                    }
                }
            }
        }

        private static string ToHex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }
    }
}
