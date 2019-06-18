using System;
using System.IO;
using System.Security.Cryptography;
using FdbServer.Infrastructure;

namespace FdbServer.Builder.Download
{
    internal static class IntegrityVerifier
    {
        public static IResult Verify(FdbServerUrl url, string fileName)
        {
            return Try.Wrap(() =>
            {
                var checksum = url.Sha512Hash;

                using (var hashAlgorithm = SHA512.Create())
                {
                    using (var destination = File.OpenRead(fileName))
                    {
                        var hash = hashAlgorithm.ComputeHash(destination);

                        var hashString = ToHex(hash);

                        if (hashString.Equals(checksum, StringComparison.OrdinalIgnoreCase))
                        {
                            return new SuccessResult();
                        }
                        else
                        {
                            return new FailureResult("File checksum did not match stored checksum.");
                        }
                    }
                }
            });
        }

        private static string ToHex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = (byte)(bytes[bx] >> 4);
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = (byte)(bytes[bx] & 0x0F);
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }
    }
}
