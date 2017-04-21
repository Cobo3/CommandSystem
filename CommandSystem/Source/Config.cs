using System.Collections.Generic;

namespace SickDev.CommandSystem{
    public static class Config{
        static List<string> _assembliesWithCommands = new List<string> {
           "CommandSystem",
        };

        internal static string[] assembliesWithCommands{
            get { return _assembliesWithCommands.ToArray(); }
        }

        public static void AddAssemblyWithCommands(string assembly) {
            _assembliesWithCommands.Add(assembly);
        }
    }
}
