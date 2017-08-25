using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.IntegrationTests
{
    public class StepGraphIntegrationTests
    {
        [Fact]
        public void GetSubGraphs_ReturnsListContainingSubgraphsWhenThereAreMultipleSubgraphs()
        {
            // Arrange
            DummyStep dummyStep1 = new DummyStep("1");

            DummyStep dummyStep2 = new DummyStep("2");
            DummyStep dummyStep3 = new DummyStep("3");
            dummyStep2.Dependents.Add(dummyStep3);
            dummyStep3.Dependencies.Add(dummyStep2);

            DummyStep dummyStep4 = new DummyStep("4");
            DummyStep dummyStep5 = new DummyStep("5");
            DummyStep dummyStep6 = new DummyStep("6");
            // Ensure that GetSubGraphs ignores edge direction
            dummyStep4.Dependencies.Add(dummyStep5); 
            dummyStep5.Dependents.Add(dummyStep4);
            dummyStep5.Dependents.Add(dummyStep6);
            dummyStep6.Dependencies.Add(dummyStep5);

            StepGraph stepGraph = new StepGraph(new[] { dummyStep1, dummyStep2, dummyStep3, dummyStep4, dummyStep5, dummyStep6 });

            // Act
            List<StepGraph> result = stepGraph.GetSubgraphs();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(dummyStep1, result[0]);
            Assert.Contains(dummyStep2, result[1]);
            Assert.Contains(dummyStep3, result[1]);
            Assert.Contains(dummyStep4, result[2]);
            Assert.Contains(dummyStep5, result[2]);
            Assert.Contains(dummyStep6, result[2]);
        }

        [Fact]
        public void GetSubGraphs_ReturnsListContainingMainGraphWhenThereAreNoSubgraphs()
        {
            // Arrange
            DummyStep dummyStep1 = new DummyStep("1");
            DummyStep dummyStep2 = new DummyStep("2");
            DummyStep dummyStep3 = new DummyStep("3");
            dummyStep1.Dependents.Add(dummyStep2);
            dummyStep2.Dependencies.Add(dummyStep1);
            dummyStep2.Dependents.Add(dummyStep3);
            dummyStep3.Dependencies.Add(dummyStep2);

            StepGraph stepGraph = new StepGraph(new[] { dummyStep1, dummyStep2, dummyStep3 });

            // Act
            List<StepGraph> result = stepGraph.GetSubgraphs();

            // Assert
            Assert.Equal(1, result.Count);
            Assert.Contains(dummyStep1, result[0]);
            Assert.Contains(dummyStep2, result[0]);
            Assert.Contains(dummyStep3, result[0]);
        }

        [Fact]
        public void GetSubGraphs_ReturnsEmptyListWhenThereAreNoNodes()
        {
            // Arrange
            StepGraph stepGraph = new StepGraph(new Step[0]);

            // Act
            List<StepGraph> result = stepGraph.GetSubgraphs();

            // Assert
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public void Run_CancelsAllTasksIfAStepThrowsAnException()
        {
            // The continuation is passed a System.Threading.CancellationToken whose IsCancellationRequested property is true.In this case, the continuation does not start, and it transitions to the System.Threading.Tasks.TaskStatus.Canceled state.
            // The continuation never runs because the condition set by its TaskContinuationOptions argument was not met. For example, if an antecedent goes into a System.Threading.Tasks.TaskStatus.Faulted state, its continuation that was passed the System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted option will not run but will transition to the Canceled state.
        }

        private class DummyStep : Step
        {
            public DummyStep(string name, IEnumerable<Step> dependencies = null) :
                base(name, dependencies)
            { }

            public override void Run(IPipelineContext pipelineContext)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
