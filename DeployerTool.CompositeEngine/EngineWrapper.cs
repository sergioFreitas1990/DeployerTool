using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeployerTool.Core;

namespace DeployerTool.CompositeEngine
{
    public class EngineWrapper : IEngineWrapper
    {
        private readonly IEngine _engine;
        private readonly Func<IScriptHandle, bool> _canExecute;

        public IEngine Engine => _engine;

        public bool CanExecute(IScriptHandle script)
        {
            return _canExecute(script);
        }

        public EngineWrapper(IEngine engine, Func<IScriptHandle, bool> canExecute)
        {
            _engine = engine;
            _canExecute = canExecute;
        }
    }
}
