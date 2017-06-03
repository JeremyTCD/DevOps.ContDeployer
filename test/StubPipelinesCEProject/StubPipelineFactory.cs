using JeremyTCD.PipelinesCE;
using JeremyTCD.PipelinesCE.PluginTools;
using PipelinesCE.StubPluginProject;

namespace PipelineCE.Tests.StubPipelinesCEProject
{
    public class StubPipelineFactory : IPipelineFactory
    {
        public Pipeline CreatePipeline()
        {
            return new Pipeline(new IStep[]
            {
                new Step<StubPlugin>(null)
            });
        }
    }
}
