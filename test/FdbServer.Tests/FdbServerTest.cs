namespace FdbServer.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Xunit;

    public class FdbServerTest
    {
        [Fact]
        public async Task TestFullCycle()
        {
            var builder = new FdbServerBuilder();
            var server = await builder.BuildAsync();

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

        [Fact]
        public async Task ClusterFile_ReturnsPath()
        {
            var builder = new FdbServerBuilder();
            var server = await builder.BuildAsync();

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
    }
}
