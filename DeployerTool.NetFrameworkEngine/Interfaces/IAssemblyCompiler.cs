using System.Threading;
using System.Threading.Tasks;

namespace DeployerTool.NetFrameworkEngine.Interfaces
{
    public interface IAssemblyCompiler
    {
        Task<IAssembly> CompileCodeAsync(string fileContents, CancellationToken cancellationToken);
    }
}
