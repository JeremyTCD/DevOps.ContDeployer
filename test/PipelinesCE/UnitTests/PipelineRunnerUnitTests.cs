using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.Tests.UnitTests
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
            StubPlugin1Options plugin1Options = new StubPlugin1Options();
            StubPlugin2Options plugin2Options = new StubPlugin2Options();
            IEnumerable<IStep> steps = new IStep[] {
                new Step<StubPlugin1>(plugin1Options),
                new Step<StubPlugin2>(plugin2Options)
            };
            PipelineOptions options = new PipelineOptions();
            Pipeline pipeline = new Pipeline(steps, options);

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();

            Mock<IPipelineContextFactory> mockPipelineContextFactory = _mockRepository.Create<IPipelineContextFactory>();
            mockPipelineContextFactory.Setup(p => p.AddPipelineOptions(options)).Returns(mockPipelineContextFactory.Object);
            mockPipelineContextFactory.Setup(p => p.CreatePipelineContext()).Returns(mockPipelineContext.Object);

            Mock<ILogger<PipelineRunner>> mockLogger = _mockRepository.Create<ILogger<PipelineRunner>>();

            Mock<IPluginFactory> mockPluginFactory = _mockRepository.Create<IPluginFactory>();
            StubPlugin1 plugin1 = new StubPlugin1();
            StubPlugin2 plugin2 = new StubPlugin2();
            mockPluginFactory.Setup(p => p.CreatePlugin(typeof(StubPlugin1))).Returns(plugin1);
            mockPluginFactory.Setup(p => p.CreatePlugin(typeof(StubPlugin2))).Returns(plugin2);

            Mock<ILogger> mockPlugin1Logger = _mockRepository.Create<ILogger>();
            Mock<ILogger> mockPlugin2Logger = _mockRepository.Create<ILogger>();

            Mock<ILoggerFactory> mockLoggerFactory = _mockRepository.Create<ILoggerFactory>();
            mockLoggerFactory.Setup(l => l.CreateLogger(typeof(StubPlugin1).Name)).Returns(mockPlugin1Logger.Object);
            mockLoggerFactory.Setup(l => l.CreateLogger(typeof(StubPlugin2).Name)).Returns(mockPlugin2Logger.Object);

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();

            Mock<IStepContextFactory> mockStepContextFactory = _mockRepository.Create<IStepContextFactory>();
            mockStepContextFactory.Setup(s => s.AddPluginOptions(plugin1Options)).Returns(mockStepContextFactory.Object);
            mockStepContextFactory.Setup(s => s.AddPluginOptions(plugin2Options)).Returns(mockStepContextFactory.Object);
            mockStepContextFactory.Setup(s => s.AddRemainingSteps(It.Is<LinkedList<IStep>>(ll => ll.Count == 1))).Returns(mockStepContextFactory.Object);
            mockStepContextFactory.Setup(s => s.AddRemainingSteps(It.Is<LinkedList<IStep>>(ll => ll.Count == 0))).Returns(mockStepContextFactory.Object);
            mockStepContextFactory.Setup(s => s.AddLogger(mockPlugin1Logger.Object)).Returns(mockStepContextFactory.Object);
            mockStepContextFactory.Setup(s => s.AddLogger(mockPlugin2Logger.Object)).Returns(mockStepContextFactory.Object);
            mockStepContextFactory.Setup(s => s.CreateStepContext()).Returns(mockStepContext.Object);

            PipelineRunner pipelineRunner = new PipelineRunner(mockLogger.Object, mockPluginFactory.Object, mockStepContextFactory.Object,
                mockPipelineContextFactory.Object, mockLoggerFactory.Object);

            // Act
            pipelineRunner.Run(pipeline);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(mockStepContext.Object, plugin1.StepContext);
            Assert.Equal(mockStepContext.Object, plugin2.StepContext);
            Assert.Equal(mockPipelineContext.Object, plugin1.PipelineContext);
            Assert.Equal(mockPipelineContext.Object, plugin2.PipelineContext);
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
            }
        }
    }
}