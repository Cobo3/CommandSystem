using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    internal static class ReflectionFinder {        
        static Type[] cache;
        
        public static Type[] LoadUserClassesAndStructs(bool reload = false) {
            if (reload || cache == null) {
                List<Type> types = new List<Type>();
                Assembly[] assemblies = GetAssembliesWithCommands();
                CommandsManager.SendMessage("Loading CommandSystem data from: " + 
                    String.Join(", ", assemblies.ToList().ConvertAll(x => x.ManifestModule.Name).ToArray()) + ".");
                for (int i = 0; i < assemblies.Length; i++)
                    types.AddRange(assemblies[i].GetTypes());
                cache = types.Where(x => x.IsClass || x.IsValueType && !x.IsEnum).ToArray();
            }
            return cache;
        }

        static Assembly[] GetAssembliesWithCommands() {
            List<Assembly> assemblies = new List<Assembly>();
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            string[] assembliesWithCommands = Config.assembliesWithCommands;

            for(int i = 0; i < assembliesWithCommands.Length; i++) {
                bool loaded = false;
                for(int j = 0; j < loadedAssemblies.Length; j++) {
                    if(loadedAssemblies[j].GetName().Name == assembliesWithCommands[i]) {
                        loaded = true;
                        assemblies.Add(loadedAssemblies[j]);
                        break;
                    }
                }
                if(!loaded) {
                    try {
                        Assembly assembly = Assembly.Load(new AssemblyName(assembliesWithCommands[i]));
                        assemblies.Add(assembly);
                    }
                    catch {
                        CommandsManager.SendMessage("Assembly with name '" + assembliesWithCommands[i] + "' could not be found. Please, make sure the assembly is properly loaded");
                    }
                }
            }
            return assemblies.ToArray();
        }
    }
}