using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeployerTool.NetFrameworkEngine.Interfaces;

namespace DeployerTool.NetFrameworkEngine
{
    public class CodeCompilerAssemblyCompiler : IAssemblyCompiler
    {
        private readonly CodeDomProvider _provider;
        private readonly CompilerParameters _compilerParameters;

        public CodeCompilerAssemblyCompiler(CodeDomProvider provider, CompilerParameters compilerParameters)
        {
            _provider = provider;
            _compilerParameters = compilerParameters;
        }

        public async Task<IAssembly> CompileCodeAsync(string fileContents, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileContents))
            {
                throw new ArgumentNullException(nameof(fileContents));
            }
            
            var compilerResults = await Task.Run(
                () => _provider.CompileAssemblyFromSource(_compilerParameters, fileContents), 
                cancellationToken);

            if (compilerResults.Errors.HasErrors)
            {
                throw new ArgumentException(
                    string.Join(Environment.NewLine, compilerResults
                        .Errors
                        .Cast<CompilerError>()
                        .Select(t => t.ToString())), 
                    nameof(fileContents));
            }

            return new AssemblyWrapper(compilerResults.CompiledAssembly);
        }

        public static CompilerParameters GetDefaultCompilerParameters(AppDomain appDomain)
        {
            if (appDomain == null)
            {
                throw new ArgumentNullException(nameof(appDomain));
            }

            var compilerParameters = new CompilerParameters
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false
            };

            compilerParameters.ReferencedAssemblies.AddRange(appDomain
                .GetAllReferenced()
                .Where(t => !t.IsDynamic)
                .Select(t => t.Location)
                .ToArray());

            return compilerParameters;
        }
    }
}
