using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.IntegrationTests
{
    public class PipelineRunnerIntegrationTests
    {
        private PipelineRunner _pipelineRunner { get; }
        private MockRepository _mockRepository { get; }

        public PipelineRunnerIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };
            Mock<ILoggerFactory> mockLoggerFactory = _mockRepository.Create<ILoggerFactory>();
            _pipelineRunner = new PipelineRunner(new StepGraphFactory(), mockLoggerFactory.Object);
        }

        [Fact]
        public void Run_IfATaskThrowsAnExceptionTasksThatHaveNotStartedAreCancelled()
        {
            // Arrange
            RunnableStep runnableStep1 = new RunnableStep("r1");
            ExceptionThrowingStep exceptionThrowingStep = new ExceptionThrowingStep("e", new[] { runnableStep1 });
            RunnableStep runnableStep2 = new RunnableStep("r2", new[] { exceptionThrowingStep });
            RunnableStep runnableStep3 = new RunnableStep("r3", new[] { runnableStep2 }); // Should get cancelled
            RunnableStep runnableStep4 = new RunnableStep("r4", new[] { runnableStep2 }, true); // Should not get cancelled

            Pipeline pipeline = new Pipeline(null, new Step[] { runnableStep1, exceptionThrowingStep, runnableStep2, runnableStep3, runnableStep4 });

            // Act and Assert
            AggregateException exception = Assert.Throws<AggregateException>(() => _pipelineRunner.Run(pipeline, null));

            // Assert
            // TODO create StepStatus enum, cast task status to step status in step status property
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep1.Task.Status);
            Assert.Equal(TaskStatus.Faulted, exceptionThrowingStep.Task.Status);
            Assert.Equal(TaskStatus.Canceled, runnableStep2.Task.Status);
            Assert.Equal(TaskStatus.Canceled, runnableStep3.Task.Status);
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep4.Task.Status);
            // TODO assert that aggregateexception is as expected (create a private dummy exception type)
            // TODO expost exception through step?  
        }

        private class RunnableStep : Step
        {
            public RunnableStep(string name, IEnumerable<Step> dependencies = null, bool runEvenIfFaultOccurs = false) :
                base(name, dependencies, runEvenIfFaultOccurs)
            { }

            public override void Run(IPipelineContext pipelineContext, CancellationToken cancellationToken)
            {
            }
        }

        private class ExceptionThrowingStep : Step
        {
            public ExceptionThrowingStep(string name, IEnumerable<Step> dependencies = null) :
                base(name, dependencies)
            { }

            public override void Run(IPipelineContext pipelineContext, CancellationToken cancellationToken)
            {
                throw new Exception();
            }
        }

    }
}
