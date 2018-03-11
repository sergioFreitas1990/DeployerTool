using System;
using DeployerTool.Core;

namespace DeployerTool.CompositeEngine
{
    public static class IEngineExtensions
    {
        public static IEngineWrapper Wrap(this IEngine engine, Func<IScriptHandle, bool> canExecute)
        {
            return new EngineWrapper(engine, canExecute);
        }
    }
}
