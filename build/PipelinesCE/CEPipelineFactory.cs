using JeremyTCD.PipelinesCE.Plugin.MSBuild;
using JeremyTCD.PipelinesCE.Core;

namespace PipelinesCE
{
    public class CEPipelineFactory : IPipelineFactory
    {
        public Pipeline CreatePipeline()
        {
            return new Pipeline(
                new IStep[]{
                    new Step<MSBuildPlugin>(new MSBuildPluginOptions
                    {
                        Switches = "/t:restore,build /p:Configuration=Release /m"
                    })
                }
            );
        }
    }
}
