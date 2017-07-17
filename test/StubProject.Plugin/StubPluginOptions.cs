using JeremyTCD.PipelinesCE.PluginAndConfigTools;

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
