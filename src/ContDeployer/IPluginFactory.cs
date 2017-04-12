using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer
{
    public interface IPluginFactory
    {
        void LoadTypes();

        IPlugin BuildPluginForPipelineStep(PipelineStep step);

        IPluginOptions BuildOptions(string name);
    }
}
