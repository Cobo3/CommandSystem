using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem{
    internal class Config{
        public string[] assembliesWithCommands{
            get { return _assembliesWithCommands.ToArray(); }
        }

        List<string> _assembliesWithCommands = new List<string> {
           "CommandSystem.dll",
           Assembly.GetEntryAssembly().ManifestModule.Name
        };

        public void AddAssemblyWithCommands(string assembly) {
            _assembliesWithCommands.Add(assembly);
        }
    }
}
