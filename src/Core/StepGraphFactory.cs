using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public class StepGraphFactory : IStepGraphFactory
    {
        public StepGraph CreateFromComposableGroup(ComposableGroup composableGroup)
        {
            StepGraph stepGraph = new StepGraph();

            composableGroup.PopulateStepGraph(stepGraph, new HashSet<Step>());

            return stepGraph;
        }
    }
}
 