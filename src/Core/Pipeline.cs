using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public class Pipeline
    {
        public IEnumerable<IStep> Steps { get; set; }
        public SharedPluginOptions Options { get; set; }

        public Pipeline(IEnumerable<IStep> steps, SharedPluginOptions options = null)
        {
            Steps = steps;
            Options = options ?? new SharedPluginOptions();
        }
    }
}
