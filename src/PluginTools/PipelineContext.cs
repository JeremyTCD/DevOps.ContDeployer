using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class PipelineContext : IPipelineContext
    {
        public IProcessService ProcessService { get; set; }
        public IDictionary<string, object> SharedData { get; set; }
        public SharedOptions SharedOptions { get; set; }

        public PipelineContext(IProcessService processService, 
            IOptions<SharedOptions> sharedOptionsAccessor)
        {
            ProcessService = processService;
            SharedData = new Dictionary<string, object>();
            SharedOptions = sharedOptionsAccessor.Value;
        }
    }
}
