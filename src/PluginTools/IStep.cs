using Microsoft.Extensions.Configuration;
using System;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public interface IStep
    {
        Type PluginType { get; set; }
        IPluginOptions PluginOptions { get; }
    }
}
