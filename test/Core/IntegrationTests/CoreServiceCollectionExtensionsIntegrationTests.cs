using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.IntegrationTests
{
    public class CoreServiceCollectionExtensionsIntegrationTests
    {
        [Fact]
        public void AddCore_ConfiguresServicesCorrectly()
        {
            // Arrange 
            ServiceCollection services = new ServiceCollection();

            // Act
            services.AddCore();

            // Assert
            ServiceDescriptorComparer comparer = new ServiceDescriptorComparer();
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IStepContextFactory, StepContextFactory>(), comparer));
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IPipelineContextFactory, PipelineContextFactory>(), comparer));
        }
    }
}
