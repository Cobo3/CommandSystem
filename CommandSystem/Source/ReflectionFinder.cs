using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem
{
	//TODO reflection provider quizás?
	internal class ReflectionFinder
	{
		//TODO make these instance members
		public static Type[] allTypes { get; private set; }
		public static Type[] enumTypes { get; private set; }

		Configuration configuration;
		NotificationsHandler notificationsHandler;
		Type[] userTypes;

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
			for (int i = 0; i < assemblies.Length; i++)
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

		//User types are those types defined in the registered assemblies
		List<Type> GetUserTypes()
		{
			List<Assembly> assemblies = GetRegisteredAssemblies();
			notificationsHandler.NotifyMessage("Loading CommandSystem data from: " + string.Join(", ", assemblies.ConvertAll(GetAssemblyFile).ToArray()) + ".");

			List<Type> types = new List<Type>();
			for (int i = 0; i < assemblies.Count; i++)
				types.AddRange(assemblies[i].GetTypes());
			return types;
		}

		List<Assembly> GetRegisteredAssemblies()
		{
			string[] registeredAssemblyNames = configuration.registeredAssemblies;
			List<Assembly> loadedAssemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			List<Assembly> assemblies = new List<Assembly>();

			for (int i = 0; i < registeredAssemblyNames.Length; i++)
			{
				Assembly loadedAssembly = FindAssemblyWithName(loadedAssemblies, registeredAssemblyNames[i]);
				if (loadedAssembly != null)
					assemblies.Add(loadedAssembly);
				else
					//Could load through Assembly.Load, but I don't think that's something the CommandSystem should be responsible for
					notificationsHandler.NotifyMessage($"Assembly with name '{registeredAssemblyNames[i]}' could not be found. Please, make sure the assembly is properly loaded");
			}
			return assemblies;
		}

		Assembly FindAssemblyWithName(List<Assembly> loadedAssemblies, string name) => loadedAssemblies.Find(x => x.GetName().Name == name);

		//TODO what if the assembly is built at runtime and does not belong to any file?
		string GetAssemblyFile(Assembly assembly)
		{
			AssemblyName name = assembly.GetName();
			string path = name.CodeBase;
			string extension = path.Substring(path.LastIndexOf('.'));
			return name.Name + extension;
		}
	}
}