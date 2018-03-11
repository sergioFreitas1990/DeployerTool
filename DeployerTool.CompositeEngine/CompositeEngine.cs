using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeployerTool.Core;

namespace DeployerTool.CompositeEngine
{
    public class CompositeEngine : IEngine
    {
        private readonly IEnumerable<IEngineWrapper> _wrappedEngines;

        public CompositeEngine(params IEngineWrapper[] wrappedEngines)
            : this((IEnumerable<IEngineWrapper>)wrappedEngines)
        {
        }

        public CompositeEngine(IEnumerable<IEngineWrapper> wrappedEngines)
        {
            _wrappedEngines = wrappedEngines;
        }

        public async Task<ExecutionResult> ExecuteAsync(IScriptHandle script, CancellationToken cancellationToken)
        {
            var engine = _wrappedEngines.FirstOrDefault(t => t.CanExecute(script));
            if (engine == null)
            {
                var scriptId = script.ScriptId;
                var engines = string.Join(",", _wrappedEngines
                    .Select(t => t.Engine.GetType().Name));

                throw new InvalidOperationException(
                    $"Script {scriptId} is not supported by these engines: [{engines}]");
            }

            return await engine.Engine.ExecuteAsync(script, cancellationToken);
        }
    }
}
