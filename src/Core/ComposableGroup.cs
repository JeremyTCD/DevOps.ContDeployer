using System.Collections;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    /// <summary>
    /// Implementors of this class facilitate clean <see cref="StepGraph"/> creation syntax.
    /// For example, the <see cref="SeriesGroup"/> sets <see cref="Step.Dependencies"/> and <see cref="Step.Dependents"/>up 
    /// according to the <see cref="Step"/>s indices within the group.
    /// </summary>
    public abstract class ComposableGroup : IComposable, IEnumerable<IComposable>, IList<IComposable>
    {
        public List<IComposable> Composables { get; } = new List<IComposable>();
        public int Count => Composables.Count;
        public bool IsReadOnly => false;

        public abstract HashSet<Step> PopulateStepGraph(StepGraph stepGraph, HashSet<Step> groupDependencies);

        public IComposable this[int index] { get => Composables[index]; set => Composables[index] = value; }

        public IEnumerator<IComposable> GetEnumerator()
        {
            return Composables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Composables.GetEnumerator();
        }

        public void Add(IComposable step)
        {
            Composables.Add(step);
        }

        public int IndexOf(IComposable item)
        {
            return Composables.IndexOf(item);
        }

        public void Insert(int index, IComposable item)
        {
            Composables.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Composables.RemoveAt(index);
        }

        public void Clear()
        {
            Composables.Clear();
        }

        public bool Contains(IComposable item)
        {
            return Composables.Contains(item);
        }

        public void CopyTo(IComposable[] array, int arrayIndex)
        {
            Composables.CopyTo(array, arrayIndex);
        }

        public bool Remove(IComposable item)
        {
            return Composables.Remove(item);
        }
    }
}
