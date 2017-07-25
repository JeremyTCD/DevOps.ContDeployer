using JeremyTCD.PipelinesCE.Core;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public interface IPipelineRunner 
    {
        void Run(Pipeline pipeline, IPipelineContext pipelineContext);
    }
}
