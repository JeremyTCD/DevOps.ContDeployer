using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class PipelineContextFactory : IPipelineContextFactory
    {
        private PipelineOptions _pipelineOptions { get; set; }

        public IPipelineContextFactory AddPipelineOptions(PipelineOptions pipelineOptions)
        {
            _pipelineOptions = pipelineOptions;
            return this;
        }

        public IPipelineContext CreatePipelineContext()
        {
            return new PipelineContext
            {
                PipelineOptions = _pipelineOptions,
                SharedData = new Dictionary<string, object>()
            };
        }
    }
}
