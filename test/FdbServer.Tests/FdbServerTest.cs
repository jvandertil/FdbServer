namespace FdbServer.Tests
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

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
                server.Start();

                server.Initialize();

                server.Stop();
            }
            finally
            {
                server.Destroy();
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
                server.Start();

                server.Initialize();

                Assert.NotNull(server.ClusterFile);
            }
            finally
            {
                server.Stop();
                server.Destroy();
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
