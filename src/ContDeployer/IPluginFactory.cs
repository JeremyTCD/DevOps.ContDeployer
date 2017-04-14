using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer
{
    public interface IPluginFactory
    {
        void LoadTypes();

        IPluginFactory SetPluginName(string pluginName);

        IPlugin Build();
    }
}
