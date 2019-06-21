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
        [InlineData(FdbServerVersion.v6_1_8)]
        public async Task TestFullCycle(FdbServerVersion version)
        {
            var fdbServer = await BuildServer(version).ConfigureAwait(false);

            try
            {
                fdbServer
                    .Start()
                    .Initialize()
                    .Stop()
                    .Destroy();
            }
            catch
            {
                fdbServer.Destruct();

                throw;
            }
        }

        [Theory]
        [InlineData(FdbServerVersion.v5_2_5)]
        [InlineData(FdbServerVersion.v6_0_15)]
        [InlineData(FdbServerVersion.v6_1_8)]
        public async Task ClusterFile_ReturnsPath(FdbServerVersion version)
        {
            var fdbServer = await BuildServer(version).ConfigureAwait(false);

            try
            {
                Assert.NotNull(fdbServer.ClusterFile);
            }
            finally
            {
                fdbServer.Destruct();
            }
        }

        private Task<IStoppedFdbServer> BuildServer(FdbServerVersion version)
        {
            var builder = new FdbServerBuilder();
            builder.WithVersion(version);

            return builder.BuildAsync();
        }
    }
}
