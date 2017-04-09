using System.Collections.Generic;

namespace SickDev.CommandSystem{
    internal class Config{
        List<string> _assembliesWithCommands = new List<string> {
           "CommandSystem.dll",
        };

        public string[] assembliesWithCommands{
            get { return _assembliesWithCommands.ToArray(); }
        }

        public void AddAssemblyWithCommands(string assembly) {
            _assembliesWithCommands.Add(assembly);
        }
    }
}
