using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JeremyTCD.PipelinesCE.Core
{
    public class StepGraph : IEnumerable<Step>
    {
        private HashSet<Step> _steps { get; set; } = new HashSet<Step>();

        public StepGraph(IEnumerable<Step> steps = null)
        {
            _steps = new HashSet<Step>(steps);
        }

        public void Run(IPipelineContext pipelineContext)
        {
            // Verify integrity of graph
            // - fix sort, contains isn't efficient?
            // - no cycles
            //      - in tsort, if a step has a child that has already been traversed, there is a cycle
            //      - https://www.youtube.com/watch?v=rKQaZuoUR4M

            // Sort steps topologically
            Sort();

            // Create CancellationTokenSource
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken cancellationToken = cts.Token;

            // Run steps
            foreach (Step step in _steps)
            { 
                if (step.Dependencies.Count == 0)
                {
                    step.Task = Task.Factory.StartNew(() => RunStep(step, pipelineContext, cts), cancellationToken);
                }
                else
                {
                    step.Task = Task.Factory.ContinueWhenAll(step.Dependencies.Select(p => p.Task).ToArray(),
                        _ => RunStep(step, pipelineContext, cts), cancellationToken);
                }

                Task.WaitAll(_steps.Select(s => s.Task).ToArray());
            }
        }

        private void RunStep(Step step, IPipelineContext pipelineContext, CancellationTokenSource cts)
        {
            try
            { 
                step.Logger.LogInformation(Strings.Log_RunningStep, step.Name);
                step.Run(pipelineContext);
                step.Logger.LogInformation(Strings.Log_FinishedRunningStep, step.Name);
            }
            finally
            {
                cts.Cancel();
            }
        }

        /// <summary>
        /// Identifies and returns subgraphs
        /// </summary>
        /// <returns>
        /// <see cref="List{StepGraph}"/> containing subgraphs
        /// </returns>
        public List<StepGraph> GetSubgraphs()
        {
            int color = 0;
            Dictionary<Step, int> stepColors = _steps.ToDictionary(s => s, s => -1);
            
            foreach(Step step in _steps)
            {
                ColorStep(step, stepColors, color++);
            }

            List<StepGraph> result = stepColors.
                GroupBy(pair => pair.Value, pair => pair.Key).
                Select(group => new StepGraph(group.ToList())).
                ToList();

            return result;
        }

        private void ColorStep(Step step, Dictionary<Step, int> stepColors, int color)
        {
            if(stepColors[step] == -1)
            {
                stepColors[step] = color;
                foreach(Step neighbour in step.Dependencies.Concat(step.Dependents))
                {
                    ColorStep(neighbour, stepColors, color);
                }
            }
        }

        /// <summary>
        /// Sorts steps topologically. Steps are ordered from roots to leaves.
        /// </summary>
        /// <exception cref="Exception">Thrown if StepGraph has one or more cycles</exception>
        public void Sort()
        {
            Stack<Step> sorted = new Stack<Step>();

            foreach (Step step in _steps)
            {
                TopologicalSort(step, sorted);
            }

            _steps = new HashSet<Step>(sorted);
        }

        private void TopologicalSort(Step step, Stack<Step> sorted)
        {
            if (sorted.Contains(step))
            {
                return;
            }

            foreach (Step child in step.Dependents)
            {
                if (!sorted.Contains(child))
                {
                    TopologicalSort(child, sorted);
                }
            }

            sorted.Push(step);
        }

        public IEnumerator<Step> GetEnumerator()
        {
            return _steps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _steps.GetEnumerator();
        }

        public void Add(Step step)
        {
            _steps.Add(step);
        }
    }
}
