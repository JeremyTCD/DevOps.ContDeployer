using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JeremyTCD.PipelinesCE.Core
{
    public abstract class Step : IComposable
    {
        public string Name { get; }
        public List<Step> Dependencies { get; }
        public List<Step> Dependents { get; }

        public Task Task { get; set; }
        public ILogger Logger { get; set; }

        public bool RunEvenIfFaultOccurs { get; }

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public DateTime Duration { get; }

        public Step(string name, IEnumerable<Step> dependencies = null, bool runEvenIfFaultOccurs = false)
        {
            Name = name;
            Dependencies = dependencies != null ? dependencies.ToList() : new List<Step>();
            Dependents = new List<Step>();
            RunEvenIfFaultOccurs = runEvenIfFaultOccurs;
        }

        public abstract void Run(IPipelineContext pipelineContext, CancellationToken cancellationToken);
    }
}
