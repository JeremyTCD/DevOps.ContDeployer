using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer
{
    public interface IPluginFactory
    {
        void LoadTypes();

        IPlugin BuildPlugin(string name, object options);

        IPluginOptions BuildOptions(string name);
    }
}
