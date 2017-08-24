using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.DependencyModel;
using Moq;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Moq.Sequences;

namespace JeremyTCD.PipelinesCE.Config.Tests.UnitTests
{
    public class ConfigLoaderUnitTests
    {
        private MockRepository _mockRepository { get; }
        private ConfigLoader _loader { get; } 

        public ConfigLoaderUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
            _loader = CreateLoader();
        }

        [Fact]
        public void LoadAssemblies_ThrowsExceptionIfMoreThanOneDepsJsonFileExistsInDirectory()
        {
            // Arrange
            DirectoryInfo stubDirectoryInfo = new DirectoryInfo("test");
            string testDirectory = stubDirectoryInfo.FullName;
            string assemblyLocation = typeof(ConfigLoader).GetTypeInfo().Assembly.Location;
            string[] stubPossibleDepsFiles = new string[2];

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(assemblyLocation)).Returns(stubDirectoryInfo);
            mockDirectoryService.Setup(d => d.GetFiles(testDirectory, "*.deps.json", SearchOption.TopDirectoryOnly)).
                Returns(stubPossibleDepsFiles);

            ConfigLoader loader = CreateLoader(directoryService: mockDirectoryService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => loader.LoadAssemblies());
            _mockRepository.VerifyAll();
            Assert.Equal(string.Format(Strings.Exception_MultipleDepsFiles, testDirectory), exception.Message);
        }

        [Fact]
        public void LoadAssemblies_ThrowsExceptionIfNoDepsJsonFileExistsInDirectory()
        {
            // Arrange
            DirectoryInfo stubDirectoryInfo = new DirectoryInfo("test");
            string testDirectory = stubDirectoryInfo.FullName;
            string assemblyLocation = typeof(ConfigLoader).GetTypeInfo().Assembly.Location;
            string[] stubPossibleDepsFiles = new string[0];

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(assemblyLocation)).Returns(stubDirectoryInfo);
            mockDirectoryService.Setup(d => d.GetFiles(testDirectory, "*.deps.json", SearchOption.TopDirectoryOnly)).
                Returns(stubPossibleDepsFiles);

            ConfigLoader loader = CreateLoader(directoryService: mockDirectoryService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => loader.LoadAssemblies());
            _mockRepository.VerifyAll();
            Assert.Equal(string.Format(Strings.Exception_NoDepsFiles, testDirectory), exception.Message);
        }

        [Fact]
        public void LoadAssemblies_LoadsAssembliesFromProjectRepresentedByProjectFileThatReferencePluginConfigAndToolsAssembly()
        {
            // Arrange
            DirectoryInfo stubDirectoryInfo = new DirectoryInfo("test");
            string testDirectory = stubDirectoryInfo.FullName;
            string assemblyLocation = typeof(ConfigLoader).GetTypeInfo().Assembly.Location;
            string testDepsFile = "testDepsFile";
            string[] stubPossibleDepsFiles = new string[] { testDepsFile };
            Assembly[] stubAssemblies = new Assembly[0];

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(assemblyLocation)).Returns(stubDirectoryInfo);
            mockDirectoryService.Setup(d => d.GetFiles(testDirectory, "*.deps.json", SearchOption.TopDirectoryOnly)).
                Returns(stubPossibleDepsFiles);

            DependencyContext stubDependencyContext = new DependencyContext(new TargetInfo("dummyFramework", "dummyRuntime", "dummyRuntimeSignature",
                false), CompilationOptions.Default, new CompilationLibrary[0], new RuntimeLibrary[0], new RuntimeFallbacks[0]);
            Mock<IDependencyContextService> mockDependencyContextService = _mockRepository.Create<IDependencyContextService>();
            mockDependencyContextService.Setup(d => d.CreateDependencyContext(testDepsFile)).Returns(stubDependencyContext);

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.Setup(a => a.CreateReferencingAssemblies(stubDependencyContext, typeof(IPlugin).GetTypeInfo().Assembly)).Returns(stubAssemblies);

            ConfigLoader loader = CreateLoader(dependencyContextService: mockDependencyContextService.Object, assemblyService: mockAssemblyService.Object,
                directoryService: mockDirectoryService.Object);

            // Act 
            IEnumerable<Assembly> result = loader.LoadAssemblies();

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(stubAssemblies, result);
        }

        [Fact]
        public void CreatePluginIoCContainers_CreatesAndPopulatesAnIOCContainerForEachPluginTypeInAssemblies()
        {
            // Arrange
            Type dummy1PluginType = typeof(Dummy1Plugin);
            Type dummy2PluginType = typeof(Dummy2Plugin);
            Type dummy1PluginStartupType = typeof(Dummy1PluginStartup);
            string dummy1PluginName = dummy1PluginType.Name;
            string dummy2PluginName = dummy2PluginType.Name;

            Assembly[] stubAssemblies = new Assembly[0];
            // One Plugin type with corresponding PluginStartup type, one without
            Type[] stubPluginTypes = new Type[] { dummy1PluginType, dummy2PluginType }; 
            Type[] stubPluginStartupTypes = new Type[] { dummy1PluginStartupType };

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPlugin))).Returns(stubPluginTypes);
            mockAssemblyService.Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPluginStartup))).Returns(stubPluginStartupTypes);

            Mock<ILoggingService<ConfigLoader>> mockLoggingService = _mockRepository.Create<ILoggingService<ConfigLoader>>();
            using (Sequence.Create())
            {
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_CreatingPluginIoCContainer, dummy1PluginName)).InSequence();
                mockLoggingService.
                    Setup(l => l.LogDebug(Strings.Log_ConfiguringPluginServices, dummy1PluginName, dummy1PluginStartupType.Name)).
                    InSequence();
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_CreatingPluginIoCContainer, dummy2PluginName)).InSequence();

                Mock<IPluginStartup> mockPluginStartup = _mockRepository.Create<IPluginStartup>();
                mockPluginStartup.Setup(p => p.ConfigureServices(It.IsAny<ServiceCollection>()));

                Mock<IActivatorService> mockActivatorService = _mockRepository.Create<IActivatorService>();
                mockActivatorService.Setup(a => a.CreateInstance(dummy1PluginStartupType)).Returns(mockPluginStartup.Object);

                Mock<IContainer> mockChildContainer = _mockRepository.Create<IContainer>();
                mockChildContainer.Setup(c => c.Configure(It.IsAny<Action<ConfigurationExpression>>()));

                Mock<IContainer> mockContainer = _mockRepository.Create<IContainer>();
                mockContainer.Setup(c => c.CreateChildContainer()).Returns(mockChildContainer.Object);

                ConfigLoader loader = CreateLoader(activatorService: mockActivatorService.Object, assemblyService: mockAssemblyService.Object, 
                    mainContainer: mockContainer.Object, loggingService: mockLoggingService.Object);

                // Act
                IDictionary<string, IContainer> result = loader.CreatePluginIoCContainers(stubAssemblies);

                // Assert
                _mockRepository.VerifyAll();
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.Equal(result[dummy1PluginName], mockChildContainer.Object);
                Assert.Equal(result[dummy2PluginName], mockChildContainer.Object);
            }
        }

        [Fact]
        public void CreatePipelineFactory_ThrowsExceptionIfConfigProjectDoesNotContainAnyIPipelineFactoryImplementations()
        {
            // Arrange
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[0];
            PipelinesCEOptions stubOptions = new PipelinesCEOptions();

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<ConfigLoader>> mockLoggingService = _mockRepository.Create<ILoggingService<ConfigLoader>>();

            ConfigLoader loader = CreateLoader(assemblyService: mockAssemblyService.Object, loggingService: mockLoggingService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => loader.CreatePipelineFactory(stubAssemblies, stubOptions));
            Assert.Equal(Strings.Exception_NoPipelineFactories, exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void CreatePipelineFactory_ThrowsExceptionIfNoPipelineIsSpecifiedAndMultiplePipelineFactoriesAreFound()
        {
            // Arrange
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = { typeof(Dummy1PipelineFactory), typeof(Dummy2PipelineFactory) };
            PipelinesCEOptions stubOptions = new PipelinesCEOptions();

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<ConfigLoader>> mockLoggingService = _mockRepository.Create<ILoggingService<ConfigLoader>>();

            ConfigLoader loader = CreateLoader(assemblyService: mockAssemblyService.Object, loggingService: mockLoggingService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => loader.CreatePipelineFactory(stubAssemblies, stubOptions));
            Assert.
                Equal(string.Format(Strings.Exception_MultiplePipelineFactories, string.Join(Environment.NewLine, stubPipelineFactoryTypes.Select(t => t.Name))),
                    exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void CreatePipelineFactory_ThrowsExceptionIfNoPipelineFactoryBuildsPipelineWithSpecifiedName()
        {
            // Arrange
            string testPipeline = _loader.PipelineFactoryPipelineName(typeof(Dummy1PipelineFactory));
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { typeof(Dummy2PipelineFactory) };
            PipelinesCEOptions stubOptions = new PipelinesCEOptions
            {
                Pipeline = testPipeline
            };

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<ConfigLoader>> mockLoggingService = _mockRepository.Create<ILoggingService<ConfigLoader>>();

            ConfigLoader loader = CreateLoader(assemblyService: mockAssemblyService.Object, loggingService: mockLoggingService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => loader.CreatePipelineFactory(stubAssemblies, stubOptions));
            Assert.Equal(string.Format(Strings.Exception_NoPipelineFactory, testPipeline), exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void CreatePipelineFactory_ThrowsExceptionIfMultiplePipelineFactoriesBuildPipelineWithSpecifiedName()
        {
            // Arrange
            string testPipeline = _loader.PipelineFactoryPipelineName(typeof(Dummy1PipelineFactory));
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { typeof(Dummy1PipelineFactory), typeof(Dummy1PipelineFactory) };
            PipelinesCEOptions stubOptions = new PipelinesCEOptions
            {
                Pipeline = testPipeline
            };

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<ConfigLoader>> mockLoggingService = _mockRepository.Create<ILoggingService<ConfigLoader>>();

            ConfigLoader loader = CreateLoader(assemblyService: mockAssemblyService.Object, loggingService: mockLoggingService.Object);

            // Act and Assert
            Exception exception = Assert.Throws<InvalidOperationException>(() => loader.CreatePipelineFactory(stubAssemblies, stubOptions));
            Assert.Equal(string.Format(Strings.Exception_MultiplePipelineFactoriesWithSameName,
                testPipeline,
                string.Join(Environment.NewLine, stubPipelineFactoryTypes.Select(t => t.FullName))),
                exception.Message);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void CreatePipelineFactory_SetsPipelineOptionsPipelineAndReturnsPipelineFactoryIfNoPipelineSpecifiedAndOnlyOnePipelineFactoryExists()
        {
            // Arrange
            Type dummy1PipelineFactoryType = typeof(Dummy1PipelineFactory);
            string resolvedPipeline = _loader.PipelineFactoryPipelineName(dummy1PipelineFactoryType);
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { dummy1PipelineFactoryType };
            PipelinesCEOptions stubOptions = new PipelinesCEOptions();
            Dummy1PipelineFactory stubDummy1PipelineFactory = new Dummy1PipelineFactory();

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<ConfigLoader>> mockLoggingService = _mockRepository.Create<ILoggingService<ConfigLoader>>();
            mockLoggingService.Setup(l => l.LogInformation(Strings.Log_ResolvedDefaultPipeline, resolvedPipeline));

            Mock<IActivatorService> mockActivatorService = _mockRepository.Create<IActivatorService>();
            mockActivatorService.Setup(a => a.CreateInstance(dummy1PipelineFactoryType)).Returns(stubDummy1PipelineFactory);

            ConfigLoader loader = CreateLoader(activatorService: mockActivatorService.Object, assemblyService: mockAssemblyService.Object, 
                loggingService: mockLoggingService.Object);

            // Act
            IPipelineFactory result = loader.CreatePipelineFactory(stubAssemblies, stubOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(stubDummy1PipelineFactory, result);
            Assert.Equal(stubOptions.Pipeline, resolvedPipeline);
        }

        [Fact]
        public void CreatePipelineFactory_ReturnsPipelineFactoryIfPipelineSpecifiedAndACorrespondingPipelineFactoryExists()
        {
            // Arrange
            Type dummy1PipelineFactoryType = typeof(Dummy1PipelineFactory);
            string pipeline = _loader.PipelineFactoryPipelineName(dummy1PipelineFactoryType);
            Assembly[] stubAssemblies = new Assembly[0];
            Type[] stubPipelineFactoryTypes = new Type[] { dummy1PipelineFactoryType };
            PipelinesCEOptions stubOptions = new PipelinesCEOptions
            {
                Pipeline = pipeline
            };
            Dummy1PipelineFactory stubDummy1PipelineFactory = new Dummy1PipelineFactory();

            Mock<IAssemblyService> mockAssemblyService = _mockRepository.Create<IAssemblyService>();
            mockAssemblyService.
                Setup(a => a.GetAssignableTypes(stubAssemblies, typeof(IPipelineFactory))).
                Returns(stubPipelineFactoryTypes);

            Mock<ILoggingService<ConfigLoader>> mockLoggingService = _mockRepository.Create<ILoggingService<ConfigLoader>>();
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_CreatingPipelineFactory, stubOptions.Pipeline));

            Mock<IActivatorService> mockActivatorService = _mockRepository.Create<IActivatorService>();
            mockActivatorService.Setup(a => a.CreateInstance(dummy1PipelineFactoryType)).Returns(stubDummy1PipelineFactory);

            ConfigLoader loader = CreateLoader(activatorService: mockActivatorService.Object, assemblyService: mockAssemblyService.Object, 
                loggingService: mockLoggingService.Object);

            // Act
            IPipelineFactory result = loader.CreatePipelineFactory(stubAssemblies, stubOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(stubDummy1PipelineFactory, result);
        }

        [Fact]
        public void Run_BuildsProjectBuildsPipelineAndCallsPipelineRunnerRun()
        {
            // Arrange
            string testPipeline = "testPipeline";
            Pipeline stubPipeline = null;// new Pipeline(null, null);
            IEnumerable<Assembly> stubAssemblies = new Assembly[0];
            IDictionary<string, IContainer> stubPluginContainers = new Dictionary<string, IContainer>();

            Mock<PipelinesCEOptions> mockPipelineOptions = _mockRepository.Create<PipelinesCEOptions>();
            mockPipelineOptions.Setup(p => p.Pipeline).Returns(testPipeline);

            Mock<ILoggingService<ConfigLoader>> mockLoggingService = _mockRepository.Create<ILoggingService<ConfigLoader>>();
            using (Sequence.Create())
            {
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_LoadingPipeline)).InSequence();
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_LoadingAssemblies)).InSequence();
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_SuccessfullyLoadedAssemblies)).InSequence();
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_CreatingPluginIoCContainers)).InSequence();
                mockLoggingService.Setup(l => l.LogDebug(Strings.Log_SuccessfullyCreatedPluginContainers)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_CreatingPipeline, testPipeline)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_SuccessfullyCreatedPipeline, testPipeline)).InSequence();
                mockLoggingService.Setup(l => l.LogInformation(Strings.Log_SuccessfullyLoadedPipeline)).InSequence();

                Mock<IPipelineFactory> mockPipelineFactory = _mockRepository.Create<IPipelineFactory>();
                mockPipelineFactory.Setup(p => p.CreatePipeline()).Returns(stubPipeline);

                Mock<ConfigLoader> mockLoader = _mockRepository.Create<ConfigLoader>(null, null, null, null, null, null, null, mockLoggingService.Object);
                mockLoader.Setup(l => l.CreatePluginIoCContainers(stubAssemblies)).Returns(stubPluginContainers);
                mockLoader.Setup(l => l.CreatePipelineFactory(stubAssemblies, mockPipelineOptions.Object)).Returns(mockPipelineFactory.Object);
                mockLoader.Setup(l => l.LoadAssemblies()).Returns(stubAssemblies);
                mockLoader.CallBase = true;

                // Act 
                (Pipeline pipeline, IDictionary<string, IContainer> pluginContainers) = mockLoader.Object.Load(mockPipelineOptions.Object);

                // Assert
                _mockRepository.VerifyAll();
                Assert.Equal(stubPipeline, pipeline);
                Assert.Equal(stubPluginContainers, pluginContainers);
            }
        }

        [Fact]
        public void PipelineFactoryPipelineName_ReturnsCorrespondingPipelineName()
        {
            // Act
            string result = _loader.PipelineFactoryPipelineName(typeof(Dummy1PipelineFactory));

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

        private ConfigLoader CreateLoader(IActivatorService activatorService = null,
            IDependencyContextService dependencyContextService = null,
            IAssemblyService assemblyService = null,
            IPathService pathService = null,
            IDirectoryService directoryService = null,
            IMSBuildService msBuildService = null,
            IContainer mainContainer = null,
            ILoggingService<ConfigLoader> loggingService = null)
        {
            return new ConfigLoader(activatorService, dependencyContextService, assemblyService, pathService, directoryService,
                msBuildService, mainContainer, loggingService);
        }
    }
}
