using System;
using System.Collections.Generic;

namespace FdbServer.Builder.Download
{
    internal static class FdbServerUrlRepository
    {
        private static readonly Dictionary<FdbServerVersion, FdbServerUrl> _repository = new Dictionary<FdbServerVersion, FdbServerUrl>
        {
            { FdbServerVersion.v5_2_5,
                new FdbServerUrl(
                    FdbServerVersion.v5_2_5,
                    new Uri(@"https://www.github.com/jvandertil/FdbServer/releases/download/v0.1.0/fdbserver-5.2.5.zip"),
                    "A7CC94FE02FFF018AEAD86AE3C8A6CE08BE9B87C57E391D9BE34F77F93EC2092352CA707D29D6A468FFEB26087A3D6314A65D4D5AAE9F47F7B0D7332577FB660")
            },
            { FdbServerVersion.v6_0_15,
                new FdbServerUrl(
                    FdbServerVersion.v6_0_15,
                    new Uri(@"https://www.github.com/jvandertil/FdbServer/releases/download/v0.4.0/fdbserver-6.0.15.zip"),
                    "1648E1574D74B781953C44C556CD75DBBA5F41E1812B54758BE808056A1E9A3C31468D5ADCCA2E5729D90A0BC2D461C5113B79C1DAC936AE717C6AA5799D131E")}
        };

        public static FdbServerUrl GetUrl(FdbServerVersion version)
        {
            return _repository[version];
        }
    }
}
