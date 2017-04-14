using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IPlugin
    {
        void Run(PipelineContext pipelineContext, PipelineStepContext pipelineStepContext);
    }
}
