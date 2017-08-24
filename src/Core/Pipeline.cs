using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public class Pipeline : SeriesGroup
    {
        private string _name { get; }
        private IEnumerable<Step> _preconfiguredSteps { get; }

        public Pipeline(string name, IEnumerable<Step> preconfiguredSteps = null)
        {
            _name = name;
            _preconfiguredSteps = preconfiguredSteps ?? new Step[0];
        }

        public override HashSet<Step> PopulateStepGraph(StepGraph stepGraph, HashSet<Step> groupDependencies)
        {
            HashSet<Step> nextComposableDependencies = base.PopulateStepGraph(stepGraph, groupDependencies);

            foreach (Step step in _preconfiguredSteps)
            {
                // Add current step to dependencies' dependants
                step.Dependencies.ForEach(p => p.Dependents.Add(step));

                // Add step to StepGraph
                stepGraph.Add(step);
            }

            // Remove steps in nextComposableDependencies that have had a preconfigured step added as a dependent
            foreach (Step step in nextComposableDependencies)
            {
                if (step.Dependents.Count != 0)
                {
                    nextComposableDependencies.Remove(step);
                }
            }

            // Add preconfigured steps with no dependants to nextComposalbeDependencies
            foreach (Step step in _preconfiguredSteps)
            {
                if (step.Dependents.Count == 0)
                {
                    nextComposableDependencies.Add(step);
                }
            }

            return nextComposableDependencies;
        }
    }
}
