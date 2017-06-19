using StructureMap;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    /// <summary>
    /// Tests to ensure that <see cref="Startup"/> handles service configuration correctly
    /// </summary>
    public class StartupIntegrationTests
    {
        [Fact]
        public void ConfigureServices_ConfiguresServicesCorrectly()
        {
            // Arrange
            Startup startup = new Startup();

            // Act
            IContainer claContainer = startup.ConfigureServices();

            // Assert
            IContainer pipelinesCEContainer = claContainer.GetInstance<IContainer>();
        }

        [Fact]
        public void ConfigureServices_GeneratedContainerDisposesCorrectly()
        {
            // Arrange
            Startup startup = new Startup();

            // Act
            IContainer claContainer = startup.ConfigureServices();

            // Assert
            IContainer pipelinesCEContainer = claContainer.GetInstance<IContainer>();
        }
    }
}
