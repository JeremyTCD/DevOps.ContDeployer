using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public class ParallelGroup : ComposableGroup
    {
        public override HashSet<Step> PopulateStepGraph(StepGraph stepGraph, HashSet<Step> groupDependencies)
        {
            HashSet<Step> nextComposableDependencies = new HashSet<Step>();

            for (int i = 0; i < Count; i++)
            {
                IComposable composable = this[i];

                if (composable is Step)
                {
                    Step step = composable as Step;

                    // Add dependencies to current step
                    step.Dependencies.AddRange(groupDependencies);
                    nextComposableDependencies.Add(step);

                    // Add current step to dependencies' dependants
                    step.Dependencies.ForEach(p => p.Dependents.Add(step));

                    // Add step to StepGraph
                    stepGraph.Add(step);
                }
                else if (composable is ComposableGroup)
                {
                    ComposableGroup composableGroup = composable as ComposableGroup;
                    nextComposableDependencies.UnionWith(composableGroup.PopulateStepGraph(stepGraph, nextComposableDependencies));
                }
            }

            return nextComposableDependencies;
        }
    }
}
