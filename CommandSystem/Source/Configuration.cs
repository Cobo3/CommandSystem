using System;
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
			for (int i = 0; i < assembliesToRegister.Length; i++)
				RegisterAssembly(assembliesToRegister[i]);
		}

		public void RegisterAssembly(string assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));
			if (assembly.Length == 0)
				throw new ArgumentException("Empty assembly names are not allowed", nameof(assembly));
			if (!registeredAssembliesList.Contains(assembly))
				registeredAssembliesList.Add(assembly);
		}
	}
}