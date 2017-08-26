using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.IntegrationTests
{
    public class ComposableGroupIntegrationTests
    {
        [Fact]
        public void PopulateStepGraph_SeriesGroupPopulatesStepGraph()
        {
            // Arrange
            ComposableGroup composableGroup = new SeriesGroup()
            {
                new DummyStep("1"),
                new DummyStep("2"),
                new DummyStep("3")
            };
            StepGraph result = new StepGraph();

            // Act
            composableGroup.PopulateStepGraph(result, new HashSet<Step>());

            // Assert
            DummyStep expectedDummyStep1 = new DummyStep("1");
            DummyStep expectedDummyStep2 = new DummyStep("2");
            DummyStep expectedDummyStep3 = new DummyStep("3");
            expectedDummyStep1.Dependents.Add(expectedDummyStep2);
            expectedDummyStep2.Dependencies.Add(expectedDummyStep1);
            expectedDummyStep2.Dependents.Add(expectedDummyStep3);
            expectedDummyStep3.Dependencies.Add(expectedDummyStep2);
            StepGraph expected = new StepGraph()
            {
                expectedDummyStep1,
                expectedDummyStep2,
                expectedDummyStep3
            };
            AssertStepsEqual(expected, result);
        }

        [Fact]
        public void PopulateStepGraph_ParallelGroupPopulatesStepGraph()
        {
            // Arrange
            ComposableGroup composableGroup = new ParallelGroup()
            {
                new DummyStep("1"),
                new DummyStep("2"),
                new DummyStep("3")
            };
            StepGraph result = new StepGraph();

            // Act
            composableGroup.PopulateStepGraph(result, new HashSet<Step>());

            // Assert
            DummyStep expectedDummyStep1 = new DummyStep("1");
            DummyStep expectedDummyStep2 = new DummyStep("2");
            DummyStep expectedDummyStep3 = new DummyStep("3");
            StepGraph expected = new StepGraph()
            {
                expectedDummyStep1,
                expectedDummyStep2,
                expectedDummyStep3
            };
            AssertStepsEqual(expected, result);
        }

        [Fact]
        public void PopulateStepGraph_SeriesGroupWithNestedParallelGroupPopulatesStepGraph()
        {
            // Arrange
            ComposableGroup composableGroup = new SeriesGroup()
            {
                new ParallelGroup() {
                    new DummyStep("1"),
                    new DummyStep("2"),
                    new DummyStep("3")
                },
                new DummyStep("4"),
                new DummyStep("5")
            };
            StepGraph result = new StepGraph();

            // Act
            composableGroup.PopulateStepGraph(result, new HashSet<Step>());

            // Assert
            DummyStep expectedDummyStep1 = new DummyStep("1");
            DummyStep expectedDummyStep2 = new DummyStep("2");
            DummyStep expectedDummyStep3 = new DummyStep("3");
            DummyStep expectedDummyStep4 = new DummyStep("4");
            DummyStep expectedDummyStep5 = new DummyStep("5");
            expectedDummyStep1.Dependents.Add(expectedDummyStep4);
            expectedDummyStep2.Dependents.Add(expectedDummyStep4);
            expectedDummyStep3.Dependents.Add(expectedDummyStep4);
            expectedDummyStep4.Dependencies.AddRange(new[] { expectedDummyStep1, expectedDummyStep2, expectedDummyStep3 });
            expectedDummyStep4.Dependents.Add(expectedDummyStep5);
            expectedDummyStep5.Dependencies.Add(expectedDummyStep4);
            StepGraph expected = new StepGraph()
            {
                expectedDummyStep1,
                expectedDummyStep2,
                expectedDummyStep3,
                expectedDummyStep4,
                expectedDummyStep5
            };
            AssertStepsEqual(expected, result);
        }

        [Fact]
        public void PopulateStepGraph_ParallelGroupWithNestedSeriesGroupPopulatesStepGraph()
        {
            // Arrange
            ComposableGroup composableGroup = new ParallelGroup()
            {
                new SeriesGroup() {
                    new DummyStep("1"),
                    new DummyStep("2"),
                    new DummyStep("3")
                },
                new DummyStep("4"),
                new DummyStep("5")
            };
            StepGraph result = new StepGraph();

            // Act
            composableGroup.PopulateStepGraph(result, new HashSet<Step>());

            // Assert
            DummyStep expectedDummyStep1 = new DummyStep("1");
            DummyStep expectedDummyStep2 = new DummyStep("2");
            DummyStep expectedDummyStep3 = new DummyStep("3");
            DummyStep expectedDummyStep4 = new DummyStep("4");
            DummyStep expectedDummyStep5 = new DummyStep("5");
            expectedDummyStep1.Dependents.Add(expectedDummyStep2);
            expectedDummyStep2.Dependencies.Add(expectedDummyStep1);
            expectedDummyStep2.Dependents.Add(expectedDummyStep3);
            expectedDummyStep3.Dependencies.Add(expectedDummyStep2);
            StepGraph expected = new StepGraph()
            {
                expectedDummyStep1,
                expectedDummyStep2,
                expectedDummyStep3,
                expectedDummyStep4,
                expectedDummyStep5
            };
            AssertStepsEqual(expected, result);
        }

        [Fact]
        public void PopulateStepGraph_SeriesGroupWithNestedSeriesGroupPopulatesStepGraph()
        {
            // Arrange
            ComposableGroup composableGroup = new SeriesGroup()
            {
                new SeriesGroup() {
                    new DummyStep("1"),
                    new DummyStep("2"),
                    new DummyStep("3")
                },
                new DummyStep("4"),
                new DummyStep("5")
            };
            StepGraph result = new StepGraph();

            // Act
            composableGroup.PopulateStepGraph(result, new HashSet<Step>());

            // Assert
            DummyStep expectedDummyStep1 = new DummyStep("1");
            DummyStep expectedDummyStep2 = new DummyStep("2");
            DummyStep expectedDummyStep3 = new DummyStep("3");
            DummyStep expectedDummyStep4 = new DummyStep("4");
            DummyStep expectedDummyStep5 = new DummyStep("5");
            expectedDummyStep1.Dependents.Add(expectedDummyStep2);
            expectedDummyStep2.Dependencies.Add(expectedDummyStep1);
            expectedDummyStep2.Dependents.Add(expectedDummyStep3);
            expectedDummyStep3.Dependencies.Add(expectedDummyStep2);
            expectedDummyStep3.Dependents.Add(expectedDummyStep4);
            expectedDummyStep4.Dependencies.Add(expectedDummyStep3);
            expectedDummyStep4.Dependents.Add(expectedDummyStep5);
            expectedDummyStep5.Dependencies.Add(expectedDummyStep4);
            StepGraph expected = new StepGraph()
            {
                expectedDummyStep1,
                expectedDummyStep2,
                expectedDummyStep3,
                expectedDummyStep4,
                expectedDummyStep5
            };
            AssertStepsEqual(expected, result);
        }

        [Fact]
        public void PopulateStepGraph_ParallelGroupWithNestedParallelGroupPopulatesStepGraph()
        {
            // Arrange
            ComposableGroup composableGroup = new ParallelGroup()
            {
                new ParallelGroup() {
                    new DummyStep("1"),
                    new DummyStep("2"),
                    new DummyStep("3")
                },
                new DummyStep("4"),
                new DummyStep("5")
            };
            StepGraph result = new StepGraph();

            // Act
            composableGroup.PopulateStepGraph(result, new HashSet<Step>());

            // Assert
            DummyStep expectedDummyStep1 = new DummyStep("1");
            DummyStep expectedDummyStep2 = new DummyStep("2");
            DummyStep expectedDummyStep3 = new DummyStep("3");
            DummyStep expectedDummyStep4 = new DummyStep("4");
            DummyStep expectedDummyStep5 = new DummyStep("5");
            StepGraph expected = new StepGraph()
            {
                expectedDummyStep1,
                expectedDummyStep2,
                expectedDummyStep3,
                expectedDummyStep4,
                expectedDummyStep5
            };
            AssertStepsEqual(expected, result);
        }

        [Fact]
        public void PopulateStepGraph_PipelineContainingPreconfiguredStepsPopulatesStepGraph()
        {
            // Arrange
            DummyStep dummyStep1 = new DummyStep("1");
            DummyStep dummyStep2 = new DummyStep("2", new[] { dummyStep1 });
            DummyStep dummyStep3 = new DummyStep("3", new[] { dummyStep2 });
            ComposableGroup composableGroup = new Pipeline(null, new[] { dummyStep1, dummyStep2, dummyStep3 });
            StepGraph result = new StepGraph();

            // Act
            composableGroup.PopulateStepGraph(result, new HashSet<Step>());

            // Assert
            DummyStep expectedDummyStep1 = new DummyStep("1");
            DummyStep expectedDummyStep2 = new DummyStep("2");
            DummyStep expectedDummyStep3 = new DummyStep("3");
            expectedDummyStep1.Dependents.Add(expectedDummyStep2);
            expectedDummyStep2.Dependencies.Add(expectedDummyStep1);
            expectedDummyStep2.Dependents.Add(expectedDummyStep3);
            expectedDummyStep3.Dependencies.Add(expectedDummyStep2);
            StepGraph expected = new StepGraph()
            {
                expectedDummyStep1,
                expectedDummyStep2,
                expectedDummyStep3
            };
            AssertStepsEqual(expected, result);
        }

        [Fact]
        public void PopulateStepGraph_SeriesGroupWithNestedPipelineContainingPreconfiguredStepsPopulatesStepGraph()
        {
            // Arrange
            DummyStep dummyStep1 = new DummyStep("1");
            DummyStep dummyStep2 = new DummyStep("2", new[] { dummyStep1 });
            DummyStep dummyStep3 = new DummyStep("3", new[] { dummyStep2 });
            ComposableGroup composableGroup = new SeriesGroup()
            {
                new Pipeline(null, new [] {dummyStep1, dummyStep2, dummyStep3 }),
                new DummyStep("4"),
                new DummyStep("5")
            };
            StepGraph result = new StepGraph();

            // Act
            composableGroup.PopulateStepGraph(result, new HashSet<Step>());

            // Assert
            DummyStep expectedDummyStep1 = new DummyStep("1");
            DummyStep expectedDummyStep2 = new DummyStep("2");
            DummyStep expectedDummyStep3 = new DummyStep("3");
            DummyStep expectedDummyStep4 = new DummyStep("4");
            DummyStep expectedDummyStep5 = new DummyStep("5");
            expectedDummyStep1.Dependents.Add(expectedDummyStep2);
            expectedDummyStep2.Dependencies.Add(expectedDummyStep1);
            expectedDummyStep2.Dependents.Add(expectedDummyStep3);
            expectedDummyStep3.Dependencies.Add(expectedDummyStep2);
            expectedDummyStep3.Dependents.Add(expectedDummyStep4);
            expectedDummyStep4.Dependencies.Add(expectedDummyStep3);
            expectedDummyStep4.Dependents.Add(expectedDummyStep5);
            expectedDummyStep5.Dependencies.Add(expectedDummyStep4);
            StepGraph expected = new StepGraph()
            {
                expectedDummyStep1,
                expectedDummyStep2,
                expectedDummyStep3,
                expectedDummyStep4,
                expectedDummyStep5
            };
            AssertStepsEqual(expected, result);
        }

        private void AssertStepsEqual(StepGraph expected, StepGraph actual)
        {
            foreach (Step expectedStep in expected)
            {
                string name = expectedStep.Name;
                Step resultStep = actual.Single(s => s.Name == name);

                Assert.Equal(expectedStep.Dependencies.Count, resultStep.Dependencies.Count);
                foreach (Step parentStep in expectedStep.Dependencies)
                {
                    Assert.NotNull(resultStep.Dependencies.SingleOrDefault(s => s.Name == parentStep.Name));
                }

                Assert.Equal(expectedStep.Dependents.Count, resultStep.Dependents.Count);
                foreach (Step childStep in expectedStep.Dependents)
                {
                    Assert.NotNull(resultStep.Dependents.SingleOrDefault(s => s.Name == childStep.Name));
                }
            }
        }

        private class DummyStep : Step
        {
            public DummyStep(string name, IEnumerable<Step> dependencies = null) :
                base(name, dependencies)
            { }

            public override void Run(IPipelineContext pipelineContext, CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
