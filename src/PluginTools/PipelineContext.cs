using LibGit2Sharp;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class PipelineContext
    {
        public IHttpManager HttpManager { get; set; }
        public IProcessManager ProcessManager { get; set; }
        public IRepository Repository { get; set; }
        public Dictionary<string, object> SharedData { get; set; }
        public LinkedList<Step> Steps { get; set; }

        public PipelineContext(IHttpManager httpManager, IProcessManager processManager, IRepository repository,
            IOptions<PipelineContextOptions> options)
        {
            HttpManager = httpManager;
            ProcessManager = processManager;
            Repository = repository;
            Steps = new LinkedList<Step>(options.Value.Steps);
            SharedData = new Dictionary<string, object>();
        }
    }
}
