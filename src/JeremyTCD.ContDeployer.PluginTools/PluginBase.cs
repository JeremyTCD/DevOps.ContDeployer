using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public abstract class PluginBase : IPlugin
    {
        public abstract IDictionary<string, object> DefaultConfig { get; set; }

        public abstract void Run(IDictionary<string, object> config, PipelineContext context, ILogger logger, LinkedList<PipelineStep> steps);

        /// <summary>
        /// Combines two <see cref="IDictionary{string, object}"/> instances, ignoring duplicates. <paramref name="primaryConfig"/> values take
        /// precedence.
        /// </summary>
        /// <param name="primaryConfig"></param>
        /// <param name="secondaryConfig"></param>
        /// <returns></returns>
        public IDictionary<string, object> CombineConfigs(IDictionary<string, object> primaryConfig, IDictionary<string, object> secondaryConfig)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            secondaryConfig.ToList().ForEach(x => result[x.Key] = x.Value);
            primaryConfig.ToList().ForEach(x => result[x.Key] = x.Value);

            return result;
        }
    }
}
