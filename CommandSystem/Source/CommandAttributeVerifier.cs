using System;
using System.Reflection;

namespace SickDev.CommandSystem
{
	internal class CommandAttributeVerifier
	{
		MethodInfo method;
		CommandAttribute attribute;

		public bool hasCommandAttribute => attribute != null;
		//TODO this is hard to read. Remove first "!"
		public bool isDeclarationSupported => method.IsStatic && !method.IsGenericMethod && !method.IsGenericMethodDefinition;

		public CommandAttributeVerifier(MethodInfo method)
		{
			this.method = method;
			object[] attributes = method.GetCustomAttributes(typeof(CommandAttribute), false);
			if (attributes.Length > 0)
				attribute = attributes[0] as CommandAttribute;
		}

		public Command ExtractCommand()
		{
			if (!isDeclarationSupported)
				throw new UnsupportedCommandDeclaration(method);

			Command command = (Command)Activator.CreateInstance(typeof(MethodInfoCommand), method);
			command.alias = attribute.alias;
			command.description = attribute.description;
			command.className = attribute.className;
			command.useClassName = attribute.useClassName || !string.IsNullOrEmpty(attribute.className);
			return command;
		}
	}
}