using JeremyTCD.PipelinesCE.Core;

namespace JeremyTCD.PipelinesCE.Tests.StubPluginProject
{
    public class StubPluginOptions : IPluginOptions
    {
        public string TestProperty { get; set; }

        public void Validate()
        {
        }
    }
}
