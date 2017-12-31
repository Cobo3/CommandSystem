using System.Collections.Generic;

namespace SickDev.CommandSystem{
    public class Configuration{
        List<string> _registeredAssemblies;
        public string[] registeredAssemblies{get { return _registeredAssemblies.ToArray(); }}

        public Configuration() {
            _registeredAssemblies = new List<string>();
            RegisterAssembly("CommandSystem");
        }

        public Configuration(params string[] assembliesToRegister):this() {
            for(int i = 0; i < assembliesToRegister.Length; i++)
                RegisterAssembly(assembliesToRegister[i]);
        }

        public void RegisterAssembly(string assembly) {
            if (!_registeredAssemblies.Contains(assembly))
                _registeredAssemblies.Add(assembly);
        }
    }
}