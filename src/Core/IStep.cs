using Microsoft.Extensions.Configuration;
using System;

namespace JeremyTCD.PipelinesCE.Core
{
    public interface IStep
    {
        Type PluginType { get; set; }
        IPluginOptions PluginOptions { get; }
    }
}
