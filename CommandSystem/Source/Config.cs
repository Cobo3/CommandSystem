using System.Collections.Generic;

namespace SickDev.CommandSystem{
    public static class Config{
        static List<string> _registeredAssemblies = new List<string> {
           "CommandSystem",
        };

        internal static string[] registeredAssemblies{
            get { return _registeredAssemblies.ToArray(); }
        }

        public static void RegisterAssembly(string assembly) {
            _registeredAssemblies.Add(assembly);
        }

        public static void UnregisterAssembly(string assembly) {
            _registeredAssemblies.Remove(assembly);
        }
    }
}
