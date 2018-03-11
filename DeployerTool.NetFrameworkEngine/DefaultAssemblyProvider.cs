using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeployerTool.Core;
using DeployerTool.NetFrameworkEngine.Interfaces;

namespace DeployerTool.NetFrameworkEngine
{
    public class DefaultAssemblyProvider : IAssemblyProvider
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private readonly IAssemblyCompiler _assemblyCompiler;
        private readonly Encoding _scriptFileEncoding;

        public DefaultAssemblyProvider(IAssemblyCompiler assemblyCompiler)
            : this(assemblyCompiler, DefaultEncoding)
        {
        }

        public DefaultAssemblyProvider(IAssemblyCompiler assemblyCompiler, Encoding scriptFileEncoding)
        {
            _assemblyCompiler = assemblyCompiler;
            _scriptFileEncoding = scriptFileEncoding;
        }

        public async Task<IAssembly> CompileCodeAsync(IScriptHandle codeFile, CancellationToken cancellationToken)
        {
            var fileContents = await GetFileContentAsync(codeFile, cancellationToken);
            return await _assemblyCompiler.CompileCodeAsync(fileContents, cancellationToken);
        }

        private async static Task<string> GetFileContentAsync(IScriptHandle script, CancellationToken cancellationToken)
        {
            using (var stream = await script.GetReadStreamAsync(cancellationToken))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
