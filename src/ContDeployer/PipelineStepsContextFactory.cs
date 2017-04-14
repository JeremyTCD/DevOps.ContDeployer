using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;

namespace JeremyTCD.ContDeployer
{
    public class PipelineStepContextFactory
    {
        private IProcessManager _processManager { get; }
        private IRepository _repository { get; }
        private PipelineStep _step { get; set; }

        public PipelineStepContextFactory(IProcessManager processManager, IRepository repository)
        {
            _processManager = processManager;
            _repository = repository;
        }

        public PipelineStepContextFactory AddPipelineStep(PipelineStep step)
        {
            _step = step;

            return this;
        }

        public PipelineStepContext Build()
        {
            return null;
        }
    }
}
