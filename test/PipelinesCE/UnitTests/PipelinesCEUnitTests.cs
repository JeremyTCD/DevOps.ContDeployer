using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Moq;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace JeremyTCD.PipelinesCE.Tests.UnitTests
{
    public class PipelinesCEUnitTests
    {
        private MockRepository _mockRepository { get; }

        public PipelinesCEUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Run_ThrowsExceptionIfPipelinesCEProjectDoesNotContainAnyIPipelineFactoryImplementations()
        {
            // Arrange
            string testProject = "testProject";
            string testProjectFile = "testProjectFile";
            string testDirectory = Path.GetFullPath("C:/testDirectory"); // Sets slashes to system default

            PipelineOptions options = new PipelineOptions
            {
                Project = testProject
            };

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.GetAbsolutePath(testProject)).Returns(testProjectFile);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();
            mockMSBuildService.Setup(m => m.Build(testProjectFile, Strings.Log_PipelinesCEProjectMSBuildSwitches));

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(new DirectoryInfo(testDirectory));

            IEnumerable<Assembly> assemblies = new Assembly[0];
            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.LoadAssembliesInDir(Path.Combine(testDirectory, "bin/Release/netcoreapp1.1"), true)).
                Returns(assemblies);
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPipelineFactory))).
                Returns(new Type[0]);

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, mockPathService.Object,
                mockDirectoryService.Object, mockMSBuildService.Object, null, null, null);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.Run(options));
            Assert.Equal(string.Format(Strings.Exception_NoPipelineFactories, testProjectFile), exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfNoPipelineIsSpecifiedAndMultiplePipelineFactoriesAreFound()
        {
            // Arrange
            string testProject = "testProject";
            string testProjectFile = "testProjectFile";
            string testDirectory = Path.GetFullPath("C:/testDirectory"); // Sets slashes to system default

            PipelineOptions options = new PipelineOptions
            {
                Project = testProject
            };

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.GetAbsolutePath(testProject)).Returns(testProjectFile);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();
            mockMSBuildService.Setup(m => m.Build(testProjectFile, Strings.Log_PipelinesCEProjectMSBuildSwitches));

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(new DirectoryInfo(testDirectory));

            IEnumerable<Assembly> assemblies = new Assembly[0];
            Type[] dummyTypes = new Type[] { typeof(Dummy1PipelineFactory), typeof(Dummy2PipelineFactory) };
            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.LoadAssembliesInDir(Path.Combine(testDirectory, "bin/Release/netcoreapp1.1"), true)).
                Returns(assemblies);
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPipelineFactory))).
                Returns(dummyTypes);

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, mockPathService.Object,
                mockDirectoryService.Object, mockMSBuildService.Object, null, null, null);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.Run(options));
            Assert.
                Equal(string.Format(Strings.Exception_MultiplePipelineFactories, string.Join("\n", dummyTypes.Select(t => t.Name))), 
                    exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfNoPipelineFactoryBuildsPipelineWithSpecifiedName()
        {
            // Arrange
            string testProject = "testProject";
            string testPipeline = "testPipeline";
            string testProjectFile = "testProjectFile";
            string testDirectory = Path.GetFullPath("C:/testDirectory"); // Sets slashes to system default

            PipelineOptions options = new PipelineOptions
            {
                Project = testProject,
                Pipeline = testPipeline
            };

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.GetAbsolutePath(testProject)).Returns(testProjectFile);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();
            mockMSBuildService.Setup(m => m.Build(testProjectFile, Strings.Log_PipelinesCEProjectMSBuildSwitches));

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(new DirectoryInfo(testDirectory));

            IEnumerable<Assembly> assemblies = new Assembly[0];
            Type[] dummyTypes = new Type[] { typeof(Dummy1PipelineFactory), typeof(Dummy2PipelineFactory) };
            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.LoadAssembliesInDir(Path.Combine(testDirectory, "bin/Release/netcoreapp1.1"), true)).
                Returns(assemblies);
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPipelineFactory))).
                Returns(dummyTypes);

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, mockPathService.Object,
                mockDirectoryService.Object, mockMSBuildService.Object, null, null, null);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.Run(options));
            Assert.Equal(string.Format(Strings.Exception_NoPipelineFactory, testPipeline), exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ConfiguresServicesAndCallsPipelineRunnerRunWithSpecifiedOptions()
        {
            // Arrange
            string testProject = "testProject";
            string testPipeline = nameof(StubPipelineFactory).Replace("PipelineFactory", "");
            string testProjectFile = "testProjectFile";
            string testDirectory = Path.GetFullPath("C:/testDirectory"); // Sets slashes to system default

            Mock<PipelineOptions> mockPipelineOptions = _mockRepository.Create<PipelineOptions>();
            mockPipelineOptions.Setup(p => p.Project).Returns(testProject);
            mockPipelineOptions.Setup(p => p.Pipeline).Returns(testPipeline);
            mockPipelineOptions.Setup(p => p.Combine(It.IsAny<PipelineOptions>())).Returns(mockPipelineOptions.Object);

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.GetAbsolutePath(testProject)).Returns(testProjectFile);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();
            mockMSBuildService.Setup(m => m.Build(testProjectFile, Strings.Log_PipelinesCEProjectMSBuildSwitches));

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(new DirectoryInfo(testDirectory));

            IEnumerable<Assembly> assemblies = new Assembly[0];
            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.LoadAssembliesInDir(Path.Combine(testDirectory, "bin/Release/netcoreapp1.1"), true)).
                Returns(assemblies);
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPipelineFactory))).
                Returns(new Type[] { typeof(StubPipelineFactory)});
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPlugin))).
                Returns(new Type[] { typeof(DummyPlugin) });
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPluginStartup))).
                Returns(new Type[] { typeof(DummyPluginStartup) });

            Mock<IPluginStartup> mockPluginStartup = _mockRepository.Create<IPluginStartup>();
            mockPluginStartup.Setup(p => p.ConfigureServices(It.IsAny<ServiceCollection>()));

            Mock<IActivatorService> mockActivatorService = _mockRepository.Create<IActivatorService>();
            mockActivatorService.Setup(a => a.CreateInstance(typeof(StubPipelineFactory))).Returns(new StubPipelineFactory());
            mockActivatorService.Setup(a => a.CreateInstance(typeof(DummyPluginStartup))).Returns(mockPluginStartup.Object);

            Mock<IContainer> mockContainer = _mockRepository.Create<IContainer>();
            mockContainer.Setup(c => c.Configure(It.IsAny<Action<ConfigurationExpression>>()));

            Mock<IPipelineRunner> mockPipelineRunner = _mockRepository.Create<IPipelineRunner>();
            mockPipelineRunner.Setup(p => p.Run(It.IsAny<Pipeline>()));

            PipelinesCE pipelinesCE = new PipelinesCE(mockActivatorService.Object, mockAssemblyService.Object, mockPathService.Object,
                mockDirectoryService.Object, mockMSBuildService.Object, mockPipelineRunner.Object, mockContainer.Object, null);

            // Act 
            pipelinesCE.Run(mockPipelineOptions.Object);

            // Assert
            _mockRepository.VerifyAll();
        }

        private class StubPipelineFactory : IPipelineFactory
        {
            public Pipeline CreatePipeline()
            {
                return new Pipeline(null);
            }
        }

        private class DummyPluginStartup : IPluginStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                throw new NotImplementedException();
            }
        }

        private class DummyPlugin : IPlugin
        {
            public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
            {
                throw new NotImplementedException();
            }
        }

        private class Dummy1PipelineFactory : IPipelineFactory
        {
            public Pipeline CreatePipeline()
            {
                throw new NotImplementedException();
            }
        }

        private class Dummy2PipelineFactory : IPipelineFactory
        {
            public Pipeline CreatePipeline()
            {
                throw new NotImplementedException();
            }
        }
    }
}
