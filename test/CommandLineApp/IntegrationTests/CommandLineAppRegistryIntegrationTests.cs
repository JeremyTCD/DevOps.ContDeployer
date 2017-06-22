using StructureMap;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    /// <summary>
    /// Tests to ensure that <see cref="Startup"/> handles service configuration correctly
    /// </summary>
    public class CommandLineAppRegistryIntegrationTests
    {
        [Fact]
        public void ConfigureServices_ConfiguresServicesCorrectly()
        {
            // Arrange

            // Act
            IContainer container = new Container(new CommandLineAppRegistry());

            // Assert
            IContainer pipelinesCEContainer = container.GetInstance<IContainer>();
        }

        [Fact]
        public void ConfigureServices_GeneratedContainerDisposesCorrectly()
        {
            // Arrange

            // Act
            IContainer claContainer = new Container(new CommandLineAppRegistry());

            // Assert
            IContainer pipelinesCEContainer = claContainer.GetInstance<IContainer>();
        }
    }
}
