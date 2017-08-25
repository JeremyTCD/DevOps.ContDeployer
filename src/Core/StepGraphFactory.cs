using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public class StepGraphFactory : IStepGraphFactory
    {
        public ILoggerFactory _loggerFactory { get; }

        public StepGraphFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public StepGraph CreateFromComposableGroup(ComposableGroup composableGroup)
        {
            StepGraph stepGraph = new StepGraph();

            composableGroup.PopulateStepGraph(stepGraph, new HashSet<Step>());

            // TODO validate step graph

            foreach(Step step in stepGraph)
            {
                step.Logger = _loggerFactory.CreateLogger(step.Name);
            }

            return stepGraph;
        }
    }
}
 