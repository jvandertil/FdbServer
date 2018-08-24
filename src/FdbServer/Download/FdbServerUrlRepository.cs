namespace FdbServer.Download
{
    using System;
    using System.Collections.Generic;

    internal static class FdbServerUrlRepository
    {
        private static readonly Dictionary<FdbServerVersion, FdbServerUrl> _repository = new Dictionary<FdbServerVersion, FdbServerUrl>
        {
            { FdbServerVersion.v5_2_5,
                new FdbServerUrl(
                    new Uri(@"https://github.com/jvandertil/FdbServer/releases/download/v0.1.0/fdbserver-5.2.5.zip"),
                    "A7CC94FE02FFF018AEAD86AE3C8A6CE08BE9B87C57E391D9BE34F77F93EC2092352CA707D29D6A468FFEB26087A3D6314A65D4D5AAE9F47F7B0D7332577FB660") }
        };

        public static FdbServerUrl GetUrl(FdbServerVersion version)
        {
            return _repository[version];
        }
    }
}
