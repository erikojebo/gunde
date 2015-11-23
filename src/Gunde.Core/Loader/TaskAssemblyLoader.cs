using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Gunde.Core.Loader
{
    public class TaskAssemblyLoader
    {
        public Task<IEnumerable<Type>> LoadTaskTypes(string assemblyPath)
        {
            return Task.Factory.StartNew(() =>
            {
                var assembly = Assembly.LoadFrom(assemblyPath);

                return assembly.GetTypes()
                    //.Where(x => typeof(IAutomationTask).IsAssignableFrom(x))
                    //.Where(x => x.GetCustomAttributes(typeof(UIExecutableAttribute), true).Any());
                    .Where(x => x.Name.EndsWith("Tasks"));
            });
        }
    }
}