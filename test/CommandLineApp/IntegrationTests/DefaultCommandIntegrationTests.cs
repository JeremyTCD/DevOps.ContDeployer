using StructureMap;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    [Collection(nameof(CommandLineAppCollection))]
    public class PipelinesCEIntegrationTests
    {
        private string _tempDir { get; }
        private IContainer _container { get; }

        public PipelinesCEIntegrationTests(CommandLineAppFixture fixture)
        {
            fixture.ResetTempDir();
            _container = fixture.GetContainer();
            _tempDir = fixture.TempDir;
        }

        [Fact]
        public void Start_ThrowsExceptionIfPipelinesCEProjectFileDoesNotExist()
        {

        }

    }
}
