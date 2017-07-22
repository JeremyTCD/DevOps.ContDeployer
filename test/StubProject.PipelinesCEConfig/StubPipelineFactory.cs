using JeremyTCD.PipelinesCE.Tools;
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
