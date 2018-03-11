using System;
using System.Collections.Generic;
using System.Reflection;
using DeployerTool.NetFrameworkEngine.Interfaces;

namespace DeployerTool.NetFrameworkEngine
{
    public class AssemblyWrapper : IAssembly
    {
        private readonly Assembly _wrappedAssembly;

        public AssemblyWrapper(Assembly assembly)
        {
            _wrappedAssembly = assembly;
        }

        public IEnumerable<Type> Types => _wrappedAssembly.GetTypes();
    }
}
