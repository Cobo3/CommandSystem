using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    internal static class ReflectionFinder {
        static Type[] _allTypes;
        static Type[] allTypes {
            get {
                if(_allTypes == null)
                    _allTypes = LoadAllTypes().ToArray();
                return _allTypes;
            }
        }

        static Type[] _userTypes;
        static Type[] userTypes {
            get {
                if(_userTypes == null)
                    _userTypes = LoadUserTypes().ToArray();
                return _userTypes;
            }
        }

        static List<Type> LoadAllTypes() {
            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();            
            for(int i = 0; i < assemblies.Length; i++)
                types.AddRange(assemblies[i].GetTypes());
            return types;
        }

        static List<Type> LoadUserTypes() {
            List<Type> types = new List<Type>();
            Assembly[] assemblies = GetAssembliesWithCommands();
            CommandsManager.SendMessage("Loading CommandSystem data from: " +
                String.Join(", ", assemblies.ToList().ConvertAll(x => {
                    AssemblyName name = x.GetName();
                    string path = name.CodeBase;
                    string extension = path.Substring(path.LastIndexOf('.'));
                    return name.Name + extension;
                }).ToArray()) + ".");
            for (int i = 0; i < assemblies.Length; i++)
                types.AddRange(assemblies[i].GetTypes());
            return types;
        }
        
        public static Type[] LoadClassesAndStructs() {
            return userTypes.Where(x => x.IsClass || x.IsValueType && !x.IsEnum).ToArray();
        }

        public static Type[] LoadEnums() {
            return allTypes.Where(x => x.IsEnum).ToArray();
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