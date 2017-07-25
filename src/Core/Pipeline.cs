using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public class Pipeline
    {
        public IEnumerable<IStep> Steps { get; set; }
        public SharedPluginOptions SharedPluginOptions { get; set; }

        public Pipeline(IEnumerable<IStep> steps, SharedPluginOptions sharedPluginOptions = null)
        {
            Steps = steps;
            SharedPluginOptions = sharedPluginOptions ?? new SharedPluginOptions();
        }
    }
}
