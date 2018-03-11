using System.Threading;
using System.Threading.Tasks;

namespace DeployerTool.Core
{
    public interface IDeployer
    {
        Task<bool> ExecuteNextAsync(CancellationToken cancellationToken);
    }
}
