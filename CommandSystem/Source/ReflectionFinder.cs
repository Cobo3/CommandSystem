using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    internal static class ReflectionFinder {        
        static Type[] cache;
        
        public static Type[] LoadUserClassesAndStructs(string[] assembliesWithCommands = null, bool reload = false) {
            if (reload || cache == null) {
                List<Type> types = new List<Type>();
                Assembly[] assemblies = GetAssembliesWithCommands(assembliesWithCommands);
                CommandsManager.SendMessage("Loading CommandSystem data from: " + 
                    String.Join(", ", assemblies.ToList().ConvertAll(x => x.ManifestModule.Name).ToArray()) + ".");
                for (int i = 0; i < assemblies.Length; i++)
                    types.AddRange(assemblies[i].GetTypes());
                cache = types.Where(x => x.IsClass || x.IsValueType && !x.IsEnum).ToArray();
            }
            return cache;
        }

        static Assembly[] GetAssembliesWithCommands(string[] assembliesWithCommands) {
            List<Assembly> assemblies = new List<Assembly>();
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            for(int i = 0; i < assembliesWithCommands.Length; i++) {
                bool loaded = false;
                for(int j = 0; j < loadedAssemblies.Length; j++) {
                    if(loadedAssemblies[j].ManifestModule.Name == assembliesWithCommands[i]) {
                        loaded = true;
                        assemblies.Add(loadedAssemblies[j]);
                        break;
                    }
                }
                if(!loaded)
                    CommandsManager.SendMessage("Assembly with name '" + assembliesWithCommands[i] + "' could not be found. Please, make sure the assembly is properly loaded");
            }
            return assemblies.ToArray();
        }
    }
}