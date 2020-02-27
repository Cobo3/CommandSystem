using System.Collections.Generic;

namespace SickDev.CommandSystem
{
    public class Configuration
    {
        List<string> _registeredAssemblies;

        public bool allowThreading { get; private set; }

        public string[] registeredAssemblies => _registeredAssemblies.ToArray();

        public Configuration(bool allowThreading) 
        {
            this.allowThreading = allowThreading;
            _registeredAssemblies = new List<string>();
            RegisterAssembly("CommandSystem");
        }

        public Configuration(bool allowThreading, params string[] assembliesToRegister):this(allowThreading) 
        {
            for(int i = 0; i < assembliesToRegister.Length; i++)
                RegisterAssembly(assembliesToRegister[i]);
        }

        public void RegisterAssembly(string assembly) 
        {
            if (!_registeredAssemblies.Contains(assembly))
                _registeredAssemblies.Add(assembly);
        }
    }
}