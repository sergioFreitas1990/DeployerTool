using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeployerTool.Core;
using DeployerTool.Core.Statuses;
using DeployerTool.NetFrameworkEngine.Interfaces;

namespace DeployerTool.NetFrameworkEngine
{
    public abstract class DotNetFrameworkEngine : IEngine
    {
        private readonly IAssemblyProvider _assemblyProvider;

        public DotNetFrameworkEngine(IAssemblyProvider assemblyProvider)
        {
            _assemblyProvider = assemblyProvider;
        }

        public async Task<ExecutionResult> ExecuteAsync(IScriptHandle script, CancellationToken cancellationToken)
        {
            if (script == null)
            {
                throw new ArgumentNullException(nameof(script));
            }
            
            var compiledAssembly = await _assemblyProvider.CompileCodeAsync(script, cancellationToken);
            var entryPoint = compiledAssembly
                .Types
                .SingleOrDefault(t => typeof(IScriptEntryPoint).IsAssignableFrom(t));

            if (entryPoint == null)
            {
                return new ExecutionResult(ExecutionResultStatus.Warnings, "No entry point found!");
            }

            if (CanSkipService(entryPoint))
            {
                return new ExecutionResult(ExecutionResultStatus.Skipped);
            }
            OnBeforeExecution(entryPoint);

            var service = (IScriptEntryPoint)GetService(entryPoint);

            try
            {
                await service.ExecuteAsync(cancellationToken);
            }
            catch (AggregateException exception)
            {
                return new ExecutionResult(ExecutionResultStatus.Errors, exception
                    .InnerExceptions
                    .Select(t => t.Message));
            }

            return new ExecutionResult(ExecutionResultStatus.Success);
        }

        protected abstract object GetService(Type type);

        protected abstract void OnBeforeExecution(Type type);

        protected abstract bool CanSkipService(Type type);
    }
}