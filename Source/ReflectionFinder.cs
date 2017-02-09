using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    internal static class ReflectionFinder {        
        static Type[] cache;

        //Load types potentially holding commands: classes and structs defined in non-excluded assemblies
        public static Type[] LoadUserClassesAndStructs(bool reload = false) {
            if (reload || cache == null) {
                Stopwatch timer = Stopwatch.StartNew();
                List<Type> types = new List<Type>();
                Assembly[] assemblies = LoadNonExcludedAssemblies();
                timer.Stop();
                CommandsManager.SendMessage("Loading CommandSystem data from \n\"" + 
                    String.Join("\",\n\"", assemblies.ToList().ConvertAll(x => x.ManifestModule.Name).ToArray()) + 
                    "\".\nLoaded in "+timer.ElapsedMilliseconds+"ms.");
                for (int i = 0; i < assemblies.Length; i++)
                    types.AddRange(assemblies[i].GetTypes());
                cache = types.Where(x => x.IsClass || x.IsValueType && !x.IsEnum).ToArray();
            }
            return cache;
        }

        static Assembly[] LoadNonExcludedAssemblies() {            
            List<Assembly> assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
            List<string> dllsToExclude = Config.dllsToExclude;
            for (int i = assemblies.Count - 1; i >= 0; i--) {
                for (int j = 0; j < dllsToExclude.Count; j++) {
                    if (assemblies[i].ManifestModule.Name == dllsToExclude[j]) {
                        assemblies.RemoveAt(i);
                        break;
                    }
                }
            }
            return assemblies.ToArray();
        }
    }
}