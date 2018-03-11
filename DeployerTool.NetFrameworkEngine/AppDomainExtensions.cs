using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeployerTool.NetFrameworkEngine
{
    public static class AppDomainExtensions
    {
        public static IEnumerable<Assembly> GetAllReferenced(this AppDomain appDomain)
        {
            if (appDomain == null)
            {
                throw new ArgumentNullException(nameof(appDomain));
            }

            return appDomain
                .GetAssemblies()
                .SelectMany(t => GetChildren(t))
                .Distinct();
        }

        public static IEnumerable<Assembly> GetChildren(Assembly parent)
        {
            IEnumerable<Assembly> currList = new[] { parent };

            var allReferencedAssemblies = parent
                .GetReferencedAssemblies()
                .Where(t => !t.IsAssemblyLoaded() && t.CodeBase != null)
                .Select(t => Assembly.Load(t));

            foreach (var curr in allReferencedAssemblies)
            {
                currList = currList.Concat(GetChildren(curr));
            }

            return currList;
        }

        private static bool IsAssemblyLoaded(this AssemblyName assemblyName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(t => t.GetName())
                .Any(t => t.FullName == assemblyName.FullName);
        }
    }
}
