using System.Threading;
using System.Threading.Tasks;

namespace DeployerTool.NetFrameworkEngine.Interfaces
{
    public interface IScriptEntryPoint
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
