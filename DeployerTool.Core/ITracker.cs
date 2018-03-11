using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeployerTool.Core
{
    public interface ITracker
    {
        Task<IScriptHandle> GetHandleAsync(IEnumerable<IScriptHandle> scriptHandles, CancellationToken cancellationToken);

        Task<RegisterResult> RegistHandleAsync(IScriptHandle handler, CancellationToken cancellationToken);
    }
}
