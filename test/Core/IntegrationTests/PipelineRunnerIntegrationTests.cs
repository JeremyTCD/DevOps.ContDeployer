using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.IntegrationTests
{
    public class PipelineRunnerIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private Mock<ILoggerFactory> _mockLoggerFactory { get; }

        public PipelineRunnerIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };
            _mockLoggerFactory = _mockRepository.Create<ILoggerFactory>();
        }

        [Fact]
        public void Run_RunsAllStepsInCorrectOrder()
        {
            // Arrange
            NeighbourStatusCheckStep runnableStep1 = new NeighbourStatusCheckStep("1");
            NeighbourStatusCheckStep runnableStep2 = new NeighbourStatusCheckStep("2");
            NeighbourStatusCheckStep runnableStep3 = new NeighbourStatusCheckStep("3", new[] { runnableStep1, runnableStep2 }); // Multiple dependencies and dependents
            NeighbourStatusCheckStep runnableStep4 = new NeighbourStatusCheckStep("4", new[] { runnableStep3 });
            NeighbourStatusCheckStep runnableStep5 = new NeighbourStatusCheckStep("5", new[] { runnableStep3 });

            Pipeline pipeline = new Pipeline(null, new Step[] { runnableStep1, runnableStep2, runnableStep3, runnableStep4, runnableStep5 });

            PipelineRunner pipelineRunner = new PipelineRunner(new StepGraphFactory(), _mockLoggerFactory.Object);

            // Act 
            pipelineRunner.Run(pipeline, null);

            // Assert
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep1.Task.Status);
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep2.Task.Status);
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep3.Task.Status);
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep4.Task.Status);
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep5.Task.Status);
        }

        private class NeighbourStatusCheckStep : Step
        {
            public NeighbourStatusCheckStep(string name, IEnumerable<Step> dependencies = null, bool runEvenIfFaultOccurs = false) :
                base(name, dependencies, runEvenIfFaultOccurs)
            { }

            public override void Run(IPipelineContext pipelineContext, CancellationToken cancellationToken)
            {
                foreach (Step step in Dependencies)
                {
                    Assert.Equal(TaskStatus.RanToCompletion, step.Task.Status);
                }

                foreach (Step step in Dependents)
                {
                    Assert.True(step.Task.Status == TaskStatus.WaitingToRun || step.Task.Status == TaskStatus.WaitingForActivation);
                }
            }
        }

        [Fact]
        public void Run_TaskStatusAreSetCorrectlyIfATaskThrowsAnException()
        {
            // Arrange
            RunnableStep runnableStep1 = new RunnableStep("r1");
            ExceptionThrowingStep exceptionThrowingStep = new ExceptionThrowingStep("e", new[] { runnableStep1 });
            RunnableStep runnableStep2 = new RunnableStep("r2", new[] { exceptionThrowingStep });
            RunnableStep runnableStep3 = new RunnableStep("r3", new[] { runnableStep2 }); // Should get cancelled
            RunnableStep runnableStep4 = new RunnableStep("r4", new[] { runnableStep2 }, true); // Should not get cancelled

            Pipeline pipeline = new Pipeline(null, new Step[] { runnableStep1, exceptionThrowingStep, runnableStep2, runnableStep3, runnableStep4 });

            PipelineRunner pipelineRunner = new PipelineRunner(new StepGraphFactory(), _mockLoggerFactory.Object);

            // Act and Assert
            AggregateException exception = Assert.Throws<AggregateException>(() => pipelineRunner.Run(pipeline, null));
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep1.Task.Status);
            Assert.Equal(TaskStatus.Faulted, exceptionThrowingStep.Task.Status);
            Assert.Equal(TaskStatus.Canceled, runnableStep2.Task.Status);
            Assert.Equal(TaskStatus.Canceled, runnableStep3.Task.Status);
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep4.Task.Status);
            Assert.Equal(3, exception.InnerExceptions.Count);
            Assert.Equal(1, exception.InnerExceptions.Where(e => e.GetType() == typeof(DummyException)).Count());
            Assert.Equal(2, exception.InnerExceptions.Where(e => e.GetType() == typeof(TaskCanceledException)).Count());
        }

        [Fact]
        public void Cancel_CancelsRun()
        {
            // Arrange
            PipelineRunner pipelineRunner = new PipelineRunner(new StepGraphFactory(), _mockLoggerFactory.Object);
            RunnableStep runnableStep1 = new RunnableStep("1");
            ActionStep runnableStep2 = new ActionStep("2", new[] { runnableStep1 }, (pipelineContext, CancellationToken) => pipelineRunner.Cancel());
            RunnableStep runnableStep3 = new RunnableStep("3", new[] { runnableStep2 });

            Pipeline pipeline = new Pipeline(null, new Step[] { runnableStep1, runnableStep2, runnableStep3 });

            // Act and Assert
            Assert.Throws<AggregateException>(() => pipelineRunner.Run(pipeline, null));
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep1.Task.Status);
            Assert.Equal(TaskStatus.RanToCompletion, runnableStep2.Task.Status);
            Assert.Equal(TaskStatus.Canceled, runnableStep3.Task.Status);
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
                throw new DummyException();
            }
        }

        private class DummyException : Exception
        {
        }
    }
}
