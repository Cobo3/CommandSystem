using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem 
{
    internal class ReflectionFinder
    {
        //TODO make these instance members
        public static Type[] allTypes { get; private set; }
        public static Type[] enumTypes { get; private set; }

        Configuration configuration;
        NotificationsHandler notificationsHandler;
        Type[] userTypes; //Types defined in the registered assemblies

        public Type[] userClassesAndStructs { get; private set; }

        static ReflectionFinder()
        {
            allTypes = GetAllTypes();
            enumTypes = allTypes.Where(x => x.IsEnum).ToArray();
        }

        static Type[] GetAllTypes() 
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();            
            for(int i = 0; i < assemblies.Length; i++)
                types.AddRange(assemblies[i].GetTypes());
            return types.ToArray();
        }

        //TODO pass assembly names instead of whole config
        public ReflectionFinder(Configuration configuration, NotificationsHandler notificationsHandler) 
        {
            this.configuration = configuration;
            this.notificationsHandler = notificationsHandler;
            userTypes = GetUserTypes().ToArray();
            userClassesAndStructs = userTypes.Where(x => x.IsClass || x.IsValueType && !x.IsEnum).ToArray();
        }

        List<Type> GetUserTypes() 
        {
            List<Type> types = new List<Type>();
            List<Assembly> assemblies = GetRegisteredAssemblies();
            notificationsHandler.NotifyMessage("Loading CommandSystem data from: " +
                string.Join(", ", assemblies.ConvertAll(GetAssemblyFile).ToArray()) + ".");
            for (int i = 0; i < assemblies.Count; i++)
                types.AddRange(assemblies[i].GetTypes());
            return types;
        }

        List<Assembly> GetRegisteredAssemblies() 
        {
            List<Assembly> assemblies = new List<Assembly>();
            List<Assembly> loadedAssemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
            string[] registeredAssemblyNames = configuration.registeredAssemblies;

            for(int i = 0; i < registeredAssemblyNames.Length; i++) 
            {
                Assembly loadedAssembly = loadedAssemblies.Find(x => x.GetName().Name == registeredAssemblyNames[i]);
                if (loadedAssembly != null)
                    assemblies.Add(loadedAssembly);
                else
                    //Could load through Assembly.Load, but I don't think that's something the CommandSystem should be responsible for
                    notificationsHandler.NotifyMessage($"Assembly with name '{registeredAssemblyNames[i]}' could not be found. Please, make sure the assembly is properly loaded");
            }
            return assemblies;
        }

        string GetAssemblyFile(Assembly assembly)
        {
            AssemblyName name = assembly.GetName();
            string path = name.CodeBase;
            string extension = path.Substring(path.LastIndexOf('.'));
            return name.Name + extension;
        }
    }
}