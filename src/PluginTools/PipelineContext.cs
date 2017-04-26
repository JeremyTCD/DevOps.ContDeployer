using LibGit2Sharp;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class PipelineContext : IPipelineContext
    {
        public IStepFactory StepFactory { get; set; }
        public IHttpManager HttpManager { get; set; }
        public IProcessManager ProcessManager { get; set; }
        public IRepository Repository { get; set; }
        public IDictionary<string, object> SharedData { get; set; }
        public LinkedList<IStep> Steps { get; set; }
        public SharedOptions SharedOptions { get; set; }

        public PipelineContext(IHttpManager httpManager, IProcessManager processManager, IRepository repository,
            IOptions<PipelineContextOptions> pipelineContextOptionsAccessor, 
            IStepFactory stepFactory, IOptions<SharedOptions> sharedOptionsAccessor)
        {
            StepFactory = stepFactory;
            HttpManager = httpManager;
            ProcessManager = processManager;
            Repository = repository;
            Steps = new LinkedList<IStep>(pipelineContextOptionsAccessor.Value.Steps);
            SharedData = new Dictionary<string, object>();
            SharedOptions = sharedOptionsAccessor.Value;
        }
    }
}
