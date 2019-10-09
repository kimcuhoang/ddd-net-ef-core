using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DDDEfCore.Infrastructures.EfCore.Common.Helpers
{
    public static class AssemblyHelpers
    {
        public static IEnumerable<Assembly> LoadFromSearchPatterns(params string[] searchPatterns)
        {
            if (searchPatterns == null || searchPatterns.Length == 0) return Enumerable.Empty<Assembly>();

            var assemblies = new HashSet<Assembly>();
            foreach (var searchPattern in searchPatterns)
            {
                var searchRegex = new Regex(searchPattern, RegexOptions.IgnoreCase);
                var moduleAssemblyFiles = DependencyContext
                    .Default
                    .RuntimeLibraries
                    .Where(x => searchRegex.IsMatch(x.Name))
                    .ToList();

                foreach (var assemblyFiles in moduleAssemblyFiles)
                    assemblies.Add(Assembly.Load(new AssemblyName(assemblyFiles.Name)));
            }

            return assemblies.ToList();
        }
    }
}
