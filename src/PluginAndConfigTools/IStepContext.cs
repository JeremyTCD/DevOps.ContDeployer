﻿using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Tools
{
    public interface IStepContext
    {
        IPluginOptions PluginOptions { get; set; }
        ILogger Logger { get; set; }
        LinkedList<IStep> RemainingSteps { get; set; }
    }
}
