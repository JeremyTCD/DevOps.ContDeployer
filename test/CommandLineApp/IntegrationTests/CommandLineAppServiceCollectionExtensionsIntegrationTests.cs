using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    /// <summary>
    /// These tests ensure that CommandLineAppRegistry behaves correctly
    /// </summary>
    public class CommandLineAppServiceCollectionExtensionsIntegrationTests
    {
        [Fact]
        public void AddCommandLineApp_ConfiguresServicesCorrectly()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();

            // Act
            services.AddCommandLineApp();

            // Assert
            ServiceDescriptorComparer comparer = new ServiceDescriptorComparer();
            Assert.True(services.Contains(ServiceDescriptor.Singleton<RunCommand, RunCommand>(), comparer));
            Assert.True(services.Contains(ServiceDescriptor.Singleton<RootCommand, RootCommand>(), comparer));
        }
    }
}
