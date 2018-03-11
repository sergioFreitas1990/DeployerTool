using System.Threading;
using System.Threading.Tasks;
using DeployerTool.Core;

namespace DeployerTool.NetFrameworkEngine.Interfaces
{
    public interface IAssemblyProvider
    {
        Task<IAssembly> CompileCodeAsync(IScriptHandle codeFile, CancellationToken cancellationToken);
    }
}
