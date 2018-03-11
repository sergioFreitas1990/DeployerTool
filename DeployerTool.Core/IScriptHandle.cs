using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DeployerTool.Core
{
    public interface IScriptHandle
    {
        string ScriptId { get; }

        Task<Stream> GetReadStreamAsync(CancellationToken cancellationToken);
    }
}
