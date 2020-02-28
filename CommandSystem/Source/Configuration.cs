using System.Collections.Generic;

namespace SickDev.CommandSystem
{
    public class Configuration
    {
        List<string> registeredAssembliesList = new List<string>();

        public bool allowThreading { get; private set; }

        public string[] registeredAssemblies => registeredAssembliesList.ToArray();

        public Configuration(bool allowThreading, params string[] assembliesToRegister)
        {
            this.allowThreading = allowThreading;
            RegisterAssembly("CommandSystem");
            for(int i = 0; i < assembliesToRegister.Length; i++)
                RegisterAssembly(assembliesToRegister[i]);
        }

        public void RegisterAssembly(string assembly) 
        {
            if (!registeredAssembliesList.Contains(assembly))
                registeredAssembliesList.Add(assembly);
        }
    }
}