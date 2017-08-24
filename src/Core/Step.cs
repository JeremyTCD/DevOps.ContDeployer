using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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

        public Step(string name, IEnumerable<Step> dependencies = null)
        {
            Name = name;
            Dependencies = dependencies != null ? dependencies.ToList() : new List<Step>();
            Dependents = new List<Step>();
        }

        public abstract void Run(IPipelineContext pipelineContext);
    }
}
