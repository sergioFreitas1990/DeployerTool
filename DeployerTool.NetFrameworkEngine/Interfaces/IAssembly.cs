using System;
using System.Collections.Generic;

namespace DeployerTool.NetFrameworkEngine.Interfaces
{
    public interface IAssembly
    {
        IEnumerable<Type> Types { get; }
    }
}