using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.PipelinesCE.Core
{
    public class StepGraph : IEnumerable<Step>
    {
        private HashSet<Step> _steps { get; set; } = new HashSet<Step>();

        public StepGraph() { }

        public StepGraph(IEnumerable<Step> steps)
        {
            _steps = new HashSet<Step>(steps);
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

            foreach (Step step in _steps)
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
            if (stepColors[step] == -1)
            {
                stepColors[step] = color;
                foreach (Step neighbour in step.Dependencies.Concat(step.Dependents))
                {
                    ColorStep(neighbour, stepColors, color);
                }
            }
        }

        /// <summary>
        /// Sorts steps topologically. Steps are ordered from roots to leaves.
        /// </summary>
        /// <exception cref="Exception">Thrown if StepGraph has one or more cycles</exception>
        public void SortTopologically()
        {
            Dictionary<Step, StepSortState> stepStates = _steps.ToDictionary(s => s, s => StepSortState.Untouched);
            Stack<Step> sortedSteps = new Stack<Step>(_steps.Count);

            foreach (Step step in _steps)
            {
                SortStep(step, stepStates, sortedSteps, new Stack<Step>());
            }

            _steps = new HashSet<Step>(sortedSteps);
        }

        private void SortStep(Step step, Dictionary<Step, StepSortState> stepStates, Stack<Step> sortedSteps, Stack<Step> currentPath)
        {
            if (stepStates[step] == StepSortState.Sorted)
            {
                return;
            }
            else if (stepStates[step] == StepSortState.Sorting)
            {
                string cycle = step.Name;
                Step currentStep = null;

                do
                {
                    currentStep = currentPath.Pop();
                    cycle = $"{currentStep.Name}->{cycle}";
                } while (currentStep != step);

                throw new Exception(cycle);
            }

            stepStates[step] = StepSortState.Sorting;
            currentPath.Push(step);

            foreach (Step child in step.Dependents)
            {
                if (!sortedSteps.Contains(child))
                {
                    SortStep(child, stepStates, sortedSteps, currentPath);
                }
            }

            currentPath.Pop();
            sortedSteps.Push(step);
            stepStates[step] = StepSortState.Sorted;
        }

        private enum StepSortState
        {
            Untouched,
            Sorting,
            Sorted
        }

        #region IEnumerable<T> member implementations
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
        #endregion
    }
}
