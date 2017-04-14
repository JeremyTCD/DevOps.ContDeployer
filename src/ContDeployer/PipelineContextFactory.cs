using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer
{
    public class PipelineContextFactory
    {
        private IProcessManager _processManager { get; }
        private IRepository _repository { get; }
        private LinkedList<PipelineStep> _steps { get; set; }

        public PipelineContextFactory(IProcessManager processManager, IRepository repository)
        {
            _processManager = processManager;
            _repository = repository;
        }

        public PipelineContextFactory AddPipelineSteps(List<PipelineStep> steps)
        {
            // Use linked list since steps will be added to and removed from start of list
            _steps = new LinkedList<PipelineStep>(steps);

            return this;
        }

        public PipelineContext Build()
        {
            PipelineContext context = new PipelineContext
            {
                ProcessManager = _processManager,
                Repository = _repository,
                SharedData = new Dictionary<string, object>(),
                PipelineSteps = _steps
            };

            return context;
        }
    }
}
