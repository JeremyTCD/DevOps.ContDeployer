using System;
using System.Collections.Generic;
using System.Threading;

namespace JeremyTCD.PipelinesCE.Core
{
    public class ActionStep : Step
    {
        private Action<IPipelineContext, CancellationToken> _action;

        public ActionStep(string name, IEnumerable<Step> dependencies, Action<IPipelineContext, CancellationToken> action) : base(name, dependencies)
        {
            _action = action;
        }

        public override void Run(IPipelineContext pipelineContext, CancellationToken cancellationToken)
        {
            _action(pipelineContext, cancellationToken);
        }
    }
}
