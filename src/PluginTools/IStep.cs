using Microsoft.Extensions.Configuration;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IStep
    {
        string PluginName { get; set; }
        IConfigurationSection Config { get; set; }
        IPluginOptions Options { get; }
    }
}
