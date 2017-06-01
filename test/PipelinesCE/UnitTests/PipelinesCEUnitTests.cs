using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

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
            mockMSBuildService.Setup(m => m.Build(testProjectFile, Strings.PipelinesCEProjectMSBuildSwitches));

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(new DirectoryInfo(testDirectory));

            IEnumerable<Assembly> assemblies = new Assembly[0];
            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.LoadAssembliesInDir(Path.Combine(testDirectory, "bin/Releases/netcoreapp1.1"), true)).
                Returns(assemblies);
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPipelineFactory))).
                Returns(new Type[0]);

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, mockPathService.Object,
                mockDirectoryService.Object, mockMSBuildService.Object, null, null);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.Run(options));
            Assert.Equal(string.Format(Strings.NoPipelineFactories, testProjectFile), exception.Message);
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
            mockMSBuildService.Setup(m => m.Build(testProjectFile, Strings.PipelinesCEProjectMSBuildSwitches));

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(new DirectoryInfo(testDirectory));

            IEnumerable<Assembly> assemblies = new Assembly[0];
            Type[] dummyTypes = new Type[] { typeof(Dummy1PipelineFactory), typeof(Dummy2PipelineFactory) };
            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.LoadAssembliesInDir(Path.Combine(testDirectory, "bin/Releases/netcoreapp1.1"), true)).
                Returns(assemblies);
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPipelineFactory))).
                Returns(dummyTypes);

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, mockPathService.Object,
                mockDirectoryService.Object, mockMSBuildService.Object, null, null);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.Run(options));
            Assert.
                Equal(string.Format(Strings.MultiplePipelineFactories, string.Join<Type>("\n", dummyTypes)), 
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
            mockMSBuildService.Setup(m => m.Build(testProjectFile, Strings.PipelinesCEProjectMSBuildSwitches));

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(new DirectoryInfo(testDirectory));

            IEnumerable<Assembly> assemblies = new Assembly[0];
            Type[] dummyTypes = new Type[] { typeof(Dummy1PipelineFactory), typeof(Dummy2PipelineFactory) };
            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.LoadAssembliesInDir(Path.Combine(testDirectory, "bin/Releases/netcoreapp1.1"), true)).
                Returns(assemblies);
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPipelineFactory))).
                Returns(dummyTypes);

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, mockPathService.Object,
                mockDirectoryService.Object, mockMSBuildService.Object, null, null);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.Run(options));
            Assert.Equal(string.Format(Strings.NoPipelineFactory, testPipeline), exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_CallsPipelineRunnerRunWithSpecifiedOptions()
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
            mockMSBuildService.Setup(m => m.Build(testProjectFile, Strings.PipelinesCEProjectMSBuildSwitches));

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(new DirectoryInfo(testDirectory));

            IEnumerable<Assembly> assemblies = new Assembly[0];
            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.LoadAssembliesInDir(Path.Combine(testDirectory, "bin/Releases/netcoreapp1.1"), true)).
                Returns(assemblies);
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(assemblies, typeof(IPipelineFactory))).
                Returns(new Type[] { typeof(StubPipelineFactory)});

            Mock<IActivatorService> mockActivatorService = _mockRepository.Create<IActivatorService>();
            mockActivatorService.Setup(a => a.CreateInstance(typeof(StubPipelineFactory))).Returns(new StubPipelineFactory());

            Mock<IPipelineRunner> mockPipelineRunner = _mockRepository.Create<IPipelineRunner>();
            mockPipelineRunner.Setup(p => p.Run(It.IsAny<Pipeline>()));

            PipelinesCE pipelinesCE = new PipelinesCE(mockActivatorService.Object, mockAssemblyService.Object, mockPathService.Object,
                mockDirectoryService.Object, mockMSBuildService.Object, mockPipelineRunner.Object, null);

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
