using System.Threading;
using System.Threading.Tasks;

namespace DeployerTool.Core
{
    public interface IEngine
    {
        Task<ExecutionResult> ExecuteAsync(IScriptHandle script, CancellationToken cancellationToken);
    }
}
