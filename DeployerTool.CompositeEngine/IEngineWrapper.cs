using DeployerTool.Core;

namespace DeployerTool.CompositeEngine
{
    public interface IEngineWrapper
    {
        IEngine Engine { get; }

        bool CanExecute(IScriptHandle script);
    }
}
