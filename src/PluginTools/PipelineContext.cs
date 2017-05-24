using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class PipelineContext : IPipelineContext
    {
        public IProcessManager ProcessManager { get; set; }
        public IDictionary<string, object> SharedData { get; set; }
        public SharedOptions SharedOptions { get; set; }

        public PipelineContext(IProcessManager processManager, 
            IOptions<SharedOptions> sharedOptionsAccessor)
        {
            ProcessManager = processManager;
            SharedData = new Dictionary<string, object>();
            SharedOptions = sharedOptionsAccessor.Value;
        }
    }
}
