using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Sequences;
using StructureMap;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.ConfigHost.Tests.UnitTests
{
    public class PipelineRunnerUnitTests
    {
        private MockRepository _mockRepository { get; }

        public PipelineRunnerUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Run_CallsPluginRunWithCorrectArgumentsForEachPlugin()
        {
            // Arrange
            string testPipeline = "testPipeline";
            string stubPlugin1Name = typeof(StubPlugin1).Name;
            string stubPlugin2Name = typeof(StubPlugin2).Name;
            StubPlugin1Options plugin1Options = new StubPlugin1Options();
            StubPlugin2Options plugin2Options = new StubPlugin2Options();
            IEnumerable<IStep> steps = new IStep[] {
                new Step<StubPlugin1>(plugin1Options),
                new Step<StubPlugin2>(plugin2Options)
            };
            PipelineOptions options = new PipelineOptions { Pipeline = testPipeline };
            Pipeline pipeline = new Pipeline(steps, options);

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();

            Mock<IPipelineContextFactory> mockPipelineContextFactory = _mockRepository.Create<IPipelineContextFactory>();
            mockPipelineContextFactory.Setup(p => p.AddPipelineOptions(options)).Returns(mockPipelineContextFactory.Object);
            mockPipelineContextFactory.Setup(p => p.CreatePipelineContext()).Returns(mockPipelineContext.Object);

            Mock<ILoggingService<PipelineRunner>> loggingService = _mockRepository.Create<ILoggingService<PipelineRunner>>();
            using (Sequence.Create())
            {
                loggingService.Setup(l => l.LogInformation(Strings.Log_RunningPipeline, testPipeline)).InSequence();
                loggingService.Setup(l => l.LogInformation(Strings.Log_RunningPlugin, stubPlugin1Name)).InSequence();
                loggingService.Setup(l => l.LogInformation(Strings.Log_FinishedRunningPlugin, stubPlugin1Name)).InSequence();
                loggingService.Setup(l => l.LogInformation(Strings.Log_RunningPlugin, stubPlugin2Name)).InSequence();
                loggingService.Setup(l => l.LogInformation(Strings.Log_FinishedRunningPlugin, stubPlugin2Name)).InSequence();
                loggingService.Setup(l => l.LogInformation(Strings.Log_FinishedRunningPipeline, testPipeline)).InSequence();

                StubPlugin1 plugin1 = new StubPlugin1();
                StubPlugin2 plugin2 = new StubPlugin2();
                Mock<IContainer> mockContainer = _mockRepository.Create<IContainer>();
                mockContainer.Setup(c => c.GetInstance(typeof(StubPlugin1))).Returns(plugin1);
                mockContainer.Setup(c => c.GetInstance(typeof(StubPlugin2))).Returns(plugin2);

                Mock<IDictionary<string, IContainer>> mockContainers = _mockRepository.Create<IDictionary<string, IContainer>>();
                mockContainers.Setup(c => c[stubPlugin1Name]).Returns(mockContainer.Object);
                mockContainers.Setup(c => c[stubPlugin2Name]).Returns(mockContainer.Object);

                Mock<ILogger> mockPlugin1Logger = _mockRepository.Create<ILogger>();
                Mock<ILogger> mockPlugin2Logger = _mockRepository.Create<ILogger>();

                Mock<ILoggerFactory> mockLoggerFactory = _mockRepository.Create<ILoggerFactory>();
                mockLoggerFactory.Setup(l => l.CreateLogger(stubPlugin1Name)).Returns(mockPlugin1Logger.Object);
                mockLoggerFactory.Setup(l => l.CreateLogger(stubPlugin2Name)).Returns(mockPlugin2Logger.Object);

                Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();

                Mock<IStepContextFactory> mockStepContextFactory = _mockRepository.Create<IStepContextFactory>();
                mockStepContextFactory.Setup(s => s.AddPluginOptions(plugin1Options)).Returns(mockStepContextFactory.Object);
                mockStepContextFactory.Setup(s => s.AddPluginOptions(plugin2Options)).Returns(mockStepContextFactory.Object);
                mockStepContextFactory.Setup(s => s.AddRemainingSteps(It.Is<LinkedList<IStep>>(ll => ll.Count == 1))).Returns(mockStepContextFactory.Object);
                mockStepContextFactory.Setup(s => s.AddRemainingSteps(It.Is<LinkedList<IStep>>(ll => ll.Count == 0))).Returns(mockStepContextFactory.Object);
                mockStepContextFactory.Setup(s => s.AddLogger(mockPlugin1Logger.Object)).Returns(mockStepContextFactory.Object);
                mockStepContextFactory.Setup(s => s.AddLogger(mockPlugin2Logger.Object)).Returns(mockStepContextFactory.Object);
                mockStepContextFactory.Setup(s => s.CreateStepContext()).Returns(mockStepContext.Object);

                PipelineRunner runner = new PipelineRunner(loggingService.Object, mockStepContextFactory.Object, mockPipelineContextFactory.Object, mockLoggerFactory.Object);

                // Act
                runner.Run(pipeline, mockContainers.Object);

                // Assert
                _mockRepository.VerifyAll();
                Assert.Equal(mockStepContext.Object, plugin1.StepContext);
                Assert.Equal(mockStepContext.Object, plugin2.StepContext);
                Assert.Equal(mockPipelineContext.Object, plugin1.PipelineContext);
                Assert.Equal(mockPipelineContext.Object, plugin2.PipelineContext);
            }
        }

        private class StubPlugin1 : IPlugin
        {
            public IPipelineContext PipelineContext { get; set; }
            public IStepContext StepContext { get; set; }

            public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
            {
                PipelineContext = pipelineContext;
                StepContext = stepContext;
            }
        }

        private class StubPlugin1Options : IPluginOptions
        {
            public void Validate()
            {
                throw new System.NotImplementedException();
            }
        }

        private class StubPlugin2 : IPlugin
        {
            public IPipelineContext PipelineContext { get; set; }
            public IStepContext StepContext { get; set; }

            public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
            {
                PipelineContext = pipelineContext;
                StepContext = stepContext;
            }
        }

        private class StubPlugin2Options : IPluginOptions
        {
            public void Validate()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
