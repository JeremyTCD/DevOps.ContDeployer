using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.IntegrationTests
{
    public class StepGraphIntegrationTests
    {
        private StepGraphFactory _stepGraphFactory { get; }

        public StepGraphIntegrationTests()
        {
            _stepGraphFactory = new StepGraphFactory();
        }

        [Fact]
        public void GetSubGraphs_ReturnsListContainingSubgraphsWhenThereAreMultipleSubgraphs()
        {
            // Arrange
            DummyStep dummyStep1 = new DummyStep("1");

            DummyStep dummyStep2 = new DummyStep("2");
            DummyStep dummyStep3 = new DummyStep("3", new[] { dummyStep2 });

            DummyStep dummyStep4 = new DummyStep("4");
            // Ensure that GetSubGraphs ignores edge direction when generating subgraphs
            DummyStep dummyStep5 = new DummyStep("5", new[] { dummyStep4 });
            DummyStep dummyStep6 = new DummyStep("6", new[] { dummyStep4 });

            StepGraph stepGraph = _stepGraphFactory.
                CreateFromComposableGroup(new Pipeline(null, new[] { dummyStep1, dummyStep2, dummyStep3, dummyStep4, dummyStep5, dummyStep6 }));

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
            DummyStep dummyStep2 = new DummyStep("2", new[] { dummyStep1 });
            DummyStep dummyStep3 = new DummyStep("3", new[] { dummyStep2 });

            StepGraph stepGraph = _stepGraphFactory.CreateFromComposableGroup(new Pipeline(null, new[] { dummyStep1, dummyStep2, dummyStep3 }));

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
        public void SortTopologically_SortsStepsTopographically()
        {
            // Arrange
            DummyStep dummyStep1 = new DummyStep("1");
            DummyStep dummyStep2 = new DummyStep("2");
            DummyStep dummyStep3 = new DummyStep("3", new[] { dummyStep1, dummyStep2 }); // This step has multiple dependencies and multiple dependents
            DummyStep dummyStep4 = new DummyStep("4", new[] { dummyStep3 });  // This step has a single dependency and a single dependent
            DummyStep dummyStep5 = new DummyStep("5", new[] { dummyStep3 });
            DummyStep dummyStep6 = new DummyStep("6", new[] { dummyStep4 });

            StepGraph stepGraph = _stepGraphFactory.
                CreateFromComposableGroup(new Pipeline(null, new[] { dummyStep5, dummyStep2, dummyStep1, dummyStep3, dummyStep4, dummyStep6 })); // Random order

            // Act
            stepGraph.SortTopologically();

            // Assert
            List<Step> sortedSteps = stepGraph.ToList();
            Assert.Equal(dummyStep1, sortedSteps[0]);
            Assert.Equal(dummyStep2, sortedSteps[1]);
            Assert.Equal(dummyStep3, sortedSteps[2]);
            Assert.Equal(dummyStep4, sortedSteps[3]);
            Assert.Equal(dummyStep6, sortedSteps[4]);
            Assert.Equal(dummyStep5, sortedSteps[5]); // SortTopologically is not deterministic. Initial order of steps affects the sorted order.
        }

        [Fact]
        public void SortTopologically_ThrowsExceptionIfGraphHasOneOrMoreCycles()
        {
            // Arrange
            DummyStep dummyStep1 = new DummyStep("1");
            DummyStep dummyStep2 = new DummyStep("2", new[] { dummyStep1 });
            DummyStep dummyStep3 = new DummyStep("3", new[] { dummyStep2 });
            dummyStep1.Dependencies.Add(dummyStep3);

            StepGraph stepGraph = _stepGraphFactory.CreateFromComposableGroup(new Pipeline(null, new[] { dummyStep2, dummyStep1, dummyStep3 })); // Random order

            // Act and Assert
            Exception exception = Assert.Throws<Exception>(() => stepGraph.SortTopologically());
            Assert.Equal($"{dummyStep2.Name}->{dummyStep3.Name}->{dummyStep1.Name}->{dummyStep2.Name}", exception.Message);
        }

        private class DummyStep : Step
        {
            public DummyStep(string name, IEnumerable<Step> dependencies = null) :
                base(name, dependencies)
            { }

            public override void Run(IPipelineContext pipelineContext, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
