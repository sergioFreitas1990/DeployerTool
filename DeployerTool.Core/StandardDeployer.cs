using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeployerTool.Core.Exceptions;
using DeployerTool.Core.Statuses;

namespace DeployerTool.Core
{
    public class StandardDeployer : IDeployer
    {
        private readonly ILogger _logger;
        private readonly IEngine _engine;
        private readonly ITracker _tracker;
        private readonly IEnumerable<IScriptHandle> _scriptHandles;

        public StandardDeployer(ILogger logger, IEngine engine, ITracker tracker, 
            IEnumerable<IScriptHandle> scriptHandles)
        {
            _logger = logger;
            _engine = engine;
            _tracker = tracker;
            _scriptHandles = scriptHandles;
        }

        public async Task<bool> ExecuteNextAsync(CancellationToken cancellationToken)
        {
            var script = await _tracker.GetHandleAsync(_scriptHandles, cancellationToken);
            if (script == null)
            {
                // There are no more scripts to run!
                _logger.Success("There are no more scripts to run.");
                return false;
            }

            _logger.Information("Running script {0}.", script.ScriptId);
            var executionResult = await _engine.ExecuteAsync(script, cancellationToken);

            _logger.Information("Ran script {0} - {1}:{2}.",
                script.ScriptId,
                executionResult.Status,
                string.Join(";", executionResult.Messages));

            if (executionResult.Status == ExecutionResultStatus.Errors)
            {
                throw new ExecutionFailedException(executionResult.Messages);
            }

            _logger.Verbose("Registering script {0}.", script.ScriptId);
            var registerResult = await _tracker.RegistHandleAsync(script, cancellationToken);

            _logger.Verbose("Registration script {0} - {1}:{2}.", 
                script.ScriptId,
                registerResult.Status,
                registerResult.Message);

            if (registerResult.Status == RegisterResultStatus.Failure)
            {
                throw new RegistrationFailedException(registerResult.Message);
            }

            return true;
        }
    }
}
