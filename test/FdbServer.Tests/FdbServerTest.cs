using System;
using System.Threading.Tasks;
using FdbServer.Builder;
using Xunit;

namespace FdbServer.Tests
{
    public class FdbServerTest
    {
        public FdbServerTest()
        {
            AppContext.SetSwitch("Switch.System.Net.DontEnableSystemDefaultTlsVersions", false);
        }

        [Theory]
        [InlineData(FdbServerVersion.v5_2_5)]
        [InlineData(FdbServerVersion.v6_0_15)]
        public async Task TestFullCycle(FdbServerVersion version)
        {
            var server = await BuildServer(version).ConfigureAwait(false);

            try
            {
                server
                    .Start()
                    .Initialize();
            }
            finally
            {
                server
                    .Stop()
                    .Destroy();
            }
        }

        [Theory]
        [InlineData(FdbServerVersion.v5_2_5)]
        [InlineData(FdbServerVersion.v6_0_15)]
        public async Task ClusterFile_ReturnsPath(FdbServerVersion version)
        {
            var server = await BuildServer(version).ConfigureAwait(false);

            try
            {
                server
                    .Start()
                    .Initialize();

                Assert.NotNull(server.ClusterFile);
            }
            finally
            {
                server
                    .Stop()
                    .Destroy();
            }
        }

        private Task<IFdbServer> BuildServer(FdbServerVersion version)
        {
            var builder = new FdbServerBuilder();
            builder.WithVersion(version);

            return builder.BuildAsync();
        }
    }
}
