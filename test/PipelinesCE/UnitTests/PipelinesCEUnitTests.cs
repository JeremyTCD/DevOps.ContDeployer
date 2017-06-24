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
using Moq.Sequences;

namespace JeremyTCD.PipelinesCE.Tests.UnitTests
{
    public class PipelinesCEUnitTests
    {
        private MockRepository _mockRepository { get; }
        private PipelinesCE _pipelinesCE { get; } = new PipelinesCE(null, null, null, null, null, null, null, null); // For access to PipelineFactoryPipelineName

        public PipelinesCEUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void CreatePluginContainers_CreatesAndPopulatesAnIOCContainerForEachPluginTypeInAssemblies()
        {
            // Arrange
            Type dummy1PluginType = typeof(Dummy1Plugin);
            Type dummy2PluginType = typeof(Dummy2Plugin);
            Type dummy1PluginStartup = typeof(Dummy1PluginStartup);
            string dummy1PluginName = dummy1PluginType.Name;
            string dummy2PluginName = dummy2PluginType.Name;

            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPluginTypes = new Type[] { dummy1PluginType, dummy2PluginType };
            Type[] stubPluginStartupTypes = new Type[] { dummy1PluginStartup };

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPlugin))).Returns(stubPluginTypes);
            mockAssemblyService.Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPluginStartup))).Returns(stubPluginStartupTypes);

            Mock<ILoggingService<PipelinesCE>> mockLoggingService = _mockRepository.Create<ILoggingService<PipelinesCE>>();
            using (Sequence.Create())
            {
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_ConfiguringPluginContainer, dummy1PluginName)).InSequence();
                mockLoggingService.
                    Setup(l => l.LogDebug(Strings.Log_ConfiguringPluginServices, dummy1PluginName, dummy1PluginStartup.Name)).
                    InSequence();
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_ConfiguringPluginContainer, dummy2PluginName)).InSequence();

                Mock<IPluginStartup> mockPluginStartup = _mockRepository.Create<IPluginStartup>();
                mockPluginStartup.Setup(p => p.ConfigureServices(It.IsAny<ServiceCollection>()));

                Mock<IActivatorService> mockActivatorService = _mockRepository.Create<IActivatorService>();
                mockActivatorService.Setup(a => a.CreateInstance(dummy1PluginStartup)).Returns(mockPluginStartup.Object);

                Mock<IContainer> mockChildContainer = _mockRepository.Create<IContainer>();
                mockChildContainer.Setup(c => c.Configure(It.IsAny<Action<ConfigurationExpression>>()));

                Mock<IContainer> mockContainer = _mockRepository.Create<IContainer>();
                mockContainer.Setup(c => c.CreateChildContainer()).Returns(mockChildContainer.Object);

                PipelinesCE pipelinesCE = new PipelinesCE(mockActivatorService.Object, mockAssemblyService.Object, null, null, null, null, mockContainer.Object,
                    mockLoggingService.Object);

                // Act
                IDictionary<string, IContainer> result = pipelinesCE.CreatePluginContainers(stubAssemblies);

                // Assert
                _mockRepository.VerifyAll();
                Assert.NotNull(result);
                Assert.Equal(result[dummy1PluginName], mockChildContainer.Object);
                Assert.Equal(result[dummy2PluginName], mockChildContainer.Object);
            }
        }

        [Fact]
        public void GetPipelineFactory_ThrowsExceptionIfPipelinesCEProjectDoesNotContainAnyIPipelineFactoryImplementations()
        {
            // Arrange
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[0];
            PipelineOptions stubOptions = new PipelineOptions();

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<PipelinesCE>> mockLoggingService = _mockRepository.Create<ILoggingService<PipelinesCE>>();
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RetrievingPipelineFactory, stubOptions.Pipeline));

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, null, null, null, null, null, mockLoggingService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.GetPipelineFactory(stubAssemblies, stubOptions));
            Assert.Equal(Strings.Exception_NoPipelineFactories, exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void GetPipelineFactory_ThrowsExceptionIfNoPipelineIsSpecifiedAndMultiplePipelineFactoriesAreFound()
        {
            // Arrange
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { typeof(Dummy1PipelineFactory), typeof(Dummy2PipelineFactory) };
            PipelineOptions stubOptions = new PipelineOptions();

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<PipelinesCE>> mockLoggingService = _mockRepository.Create<ILoggingService<PipelinesCE>>();
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RetrievingPipelineFactory, stubOptions.Pipeline));

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, null, null, null, null, null, mockLoggingService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.GetPipelineFactory(stubAssemblies, stubOptions));
            Assert.
                Equal(string.Format(Strings.Exception_MultiplePipelineFactories, string.Join(Environment.NewLine, stubPipelineFactoryTypes.Select(t => t.Name))),
                    exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void GetPipelineFactory_ThrowsExceptionIfNoPipelineFactoryBuildsPipelineWithSpecifiedName()
        {
            // Arrange
            string testPipeline = _pipelinesCE.PipelineFactoryPipelineName(typeof(Dummy1PipelineFactory));
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { typeof(Dummy2PipelineFactory) };
            PipelineOptions stubOptions = new PipelineOptions
            {
                Pipeline = testPipeline
            };

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<PipelinesCE>> mockLoggingService = _mockRepository.Create<ILoggingService<PipelinesCE>>();
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RetrievingPipelineFactory, stubOptions.Pipeline));

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, null, null, null, null, null, mockLoggingService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.GetPipelineFactory(stubAssemblies, stubOptions));
            Assert.Equal(string.Format(Strings.Exception_NoPipelineFactory, testPipeline), exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void GetPipelineFactory_ThrowsExceptionIfMultiplePipelineFactoriesBuildPipelineWithSpecifiedName()
        {
            // Arrange
            string testPipeline = _pipelinesCE.PipelineFactoryPipelineName(typeof(Dummy1PipelineFactory));
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { typeof(Dummy1PipelineFactory), typeof(Dummy1PipelineFactory) };
            PipelineOptions stubOptions = new PipelineOptions
            {
                Pipeline = testPipeline
            };

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<PipelinesCE>> mockLoggingService = _mockRepository.Create<ILoggingService<PipelinesCE>>();
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RetrievingPipelineFactory, stubOptions.Pipeline));

            PipelinesCE pipelinesCE = new PipelinesCE(null, mockAssemblyService.Object, null, null, null, null, null, mockLoggingService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => pipelinesCE.GetPipelineFactory(stubAssemblies, stubOptions));
            Assert.Equal(string.Format(Strings.Exception_MultiplePipelineFactoriesWithSameName,
                testPipeline,
                string.Join(Environment.NewLine, stubPipelineFactoryTypes.Select(t => t.FullName))),
                exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void GetPipelineFactory_SetsPipelineAndReturnsPipelineFactoryIfNoPipelineSpecifiedAndOnlyOnePipelineFactoryExists()
        {
            // Arrange
            Type dummy1PipelineFactory = typeof(Dummy1PipelineFactory);
            string finalPipeline = _pipelinesCE.PipelineFactoryPipelineName(dummy1PipelineFactory);
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { dummy1PipelineFactory };
            PipelineOptions stubOptions = new PipelineOptions();
            Dummy1PipelineFactory stubDummy1PipelineFactory = new Dummy1PipelineFactory();

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<PipelinesCE>> mockLoggingService = _mockRepository.Create<ILoggingService<PipelinesCE>>();
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RetrievingPipelineFactory, stubOptions.Pipeline));
            mockLoggingService.Setup(l => l.LogInformation(Strings.Log_ResolvedDefaultPipeline, finalPipeline));

            Mock<IActivatorService> mockActivatorService = _mockRepository.Create<IActivatorService>();
            mockActivatorService.Setup(a => a.CreateInstance(dummy1PipelineFactory)).Returns(stubDummy1PipelineFactory);

            PipelinesCE pipelinesCE = new PipelinesCE(mockActivatorService.Object, mockAssemblyService.Object, null, null, null, null, null, mockLoggingService.Object);

            // Act
            IPipelineFactory result = pipelinesCE.GetPipelineFactory(stubAssemblies, stubOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(stubDummy1PipelineFactory, result);
            Assert.Equal(stubOptions.Pipeline, finalPipeline);
        }

        [Fact]
        public void GetPipelineFactory_ReturnsPipelineFactoryIfPipelineSpecifiedAndACorrespondingPipelineFactoryExists()
        {
            // Arrange
            Type dummy1PipelineFactory = typeof(Dummy1PipelineFactory);
            string pipeline = _pipelinesCE.PipelineFactoryPipelineName(dummy1PipelineFactory);
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { dummy1PipelineFactory };
            PipelineOptions stubOptions = new PipelineOptions
            {
                Pipeline = pipeline
            };
            Dummy1PipelineFactory stubDummy1PipelineFactory = new Dummy1PipelineFactory();

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<PipelinesCE>> mockLoggingService = _mockRepository.Create<ILoggingService<PipelinesCE>>();
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RetrievingPipelineFactory, stubOptions.Pipeline));

            Mock<IActivatorService> mockActivatorService = _mockRepository.Create<IActivatorService>();
            mockActivatorService.Setup(a => a.CreateInstance(dummy1PipelineFactory)).Returns(stubDummy1PipelineFactory);

            PipelinesCE pipelinesCE = new PipelinesCE(mockActivatorService.Object, mockAssemblyService.Object, null, null, null, null, null, mockLoggingService.Object);

            // Act
            IPipelineFactory result = pipelinesCE.GetPipelineFactory(stubAssemblies, stubOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(stubDummy1PipelineFactory, result);
        }

        [Fact]
        public void Run_BuildsProjectBuildPipelineAndCallsPipelineRunnerRun()
        {
            // Arrange
            Type dummy1PluginStartup = typeof(Dummy1PluginStartup);
            Type dummy1PluginType = typeof(Dummy1Plugin);
            string testProject = "testProject";
            string testPipeline = "testPipeline";
            string testProjectFile = "testProjectFile";
            string testDirectory = "testDirectory";
            Pipeline stubPipeline = new Pipeline(null);
            DirectoryInfo stubDirectoryInfo = new DirectoryInfo(testDirectory);
            string assembliesDirectory = Path.Combine(stubDirectoryInfo.FullName, "bin/Release/netcoreapp1.1");
            IEnumerable<Assembly> stubAssemblies = new Assembly[0];

            Mock<PipelineOptions> mockPipelineOptions = _mockRepository.Create<PipelineOptions>();
            mockPipelineOptions.Setup(p => p.Project).Returns(testProject);
            mockPipelineOptions.Setup(p => p.Pipeline).Returns(testPipeline);
            mockPipelineOptions.Setup(p => p.Combine(It.IsAny<PipelineOptions>())).Returns(mockPipelineOptions.Object);

            Mock<ILoggingService<PipelinesCE>> mockLoggingService = _mockRepository.Create<ILoggingService<PipelinesCE>>();
            using (Sequence.Create())
            {
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_InitializingPipelinesCE)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_BuildingPipelinesCEProject, testProjectFile)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_PipelinesCEProjectSuccessfullyBuilt, testProjectFile)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_LoadingAssemblies, assembliesDirectory)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_AssembliesSuccessfullyLoaded, assembliesDirectory)).InSequence();
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_BuildingPluginContainers)).InSequence();
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_PluginContainersSuccessfullyBuilt)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_BuildingPipeline, testPipeline)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_PipelineSuccessfullyBuilt, testPipeline)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_PipelinesCESuccessfullyInitialized)).InSequence();

                Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
                mockPathService.Setup(p => p.GetAbsolutePath(testProject)).Returns(testProjectFile);

                Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
                mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(stubDirectoryInfo);

                Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();
                mockMSBuildService.Setup(m => m.Build(testProjectFile, "/t:restore,build /p:Configuration=Release"));

                Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
                mockAssemblyService.
                    Setup(a => a.LoadAssembliesInDir(assembliesDirectory, true)).
                    Returns(stubAssemblies);

                Mock<IPipelineFactory> mockPipelineFactory = _mockRepository.Create<IPipelineFactory>();
                mockPipelineFactory.Setup(p => p.CreatePipeline()).Returns(stubPipeline);

                Mock<IPipelineRunner> mockPipelineRunner = _mockRepository.Create<IPipelineRunner>();
                mockPipelineRunner.Setup(p => p.Run(stubPipeline, It.IsAny<IDictionary<string, IContainer>>()));

                Mock<PipelinesCE> pipelinesCE = _mockRepository.Create<PipelinesCE>(null, mockAssemblyService.Object, mockPathService.Object,
                    mockDirectoryService.Object, mockMSBuildService.Object, mockPipelineRunner.Object, null, mockLoggingService.Object);
                pipelinesCE.Setup(p => p.CreatePluginContainers(stubAssemblies));
                pipelinesCE.Setup(p => p.GetPipelineFactory(stubAssemblies, mockPipelineOptions.Object)).Returns(mockPipelineFactory.Object);
                pipelinesCE.CallBase = true;

                // Act 
                pipelinesCE.Object.Run(mockPipelineOptions.Object);

                // Assert
                _mockRepository.VerifyAll();
            }
        }

        [Fact]
        public void PipelineFactoryPipelineName_ReturnsCorrespondingPipelineName()
        {
            // Act
            string result = _pipelinesCE.PipelineFactoryPipelineName(typeof(Dummy1PipelineFactory));

            // Assert
            Assert.Equal("Dummy1", result);
        }

        private class Dummy1PluginStartup : IPluginStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                throw new NotImplementedException();
            }
        }

        private class Dummy1Plugin : IPlugin
        {
            public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
            {
                throw new NotImplementedException();
            }
        }

        private class Dummy2Plugin : IPlugin
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
