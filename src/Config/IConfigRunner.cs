using JeremyTCD.PipelinesCE.Core;

namespace JeremyTCD.PipelinesCE.Config
{
    public interface IConfigRunner
    {
        void Run(PipelinesCEOptions pipelinesCEOptions, SharedStepOptions sharedStepOptions);
    }
}
