using Microsoft.Extensions.Configuration;
using System;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IStep
    {
        Type PluginType { get; set; }
        IPluginOptions PluginOptions { get; }
    }
}
