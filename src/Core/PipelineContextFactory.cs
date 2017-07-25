using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public class PipelineContextFactory : IPipelineContextFactory
    {
        private PipelinesCEOptions _pipelineOptions { get; set; }

        public IPipelineContextFactory AddPipelineOptions(PipelinesCEOptions pipelineOptions)
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
