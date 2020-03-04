using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SickDev.CommandSystem
{
	public class Command
	{
		Delegate deleg;
		string _className;

		public MethodInfo method { get; private set; }
		public Signature signature { get; private set; }
		public bool isAnonymous { get; private set; }
		public bool useClassName { get; set; }
		public string alias { get; set; }
		public string description { get; set; }

		public string className
		{
			get => string.IsNullOrWhiteSpace(_className) ? method.DeclaringType.Name : _className;
			set
			{
				_className = value;
				useClassName = true;
			}
		}

		public string name
		{
			get
			{
				string name = string.Empty;
				if (useClassName)
					name = className + ".";
				name += string.IsNullOrWhiteSpace(alias) ? method.Name : alias;
				return name;
			}
		}

		public bool hasReturnValue => method.ReturnType != typeof(void);

		public Command(Delegate _deleg)
		{
			deleg = _deleg;
			method = deleg.Method;
			isAnonymous = method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
			signature = new Signature(this);
			description = string.Empty;
		}

		public bool IsOverloadOf(ParsedCommand parsedCommand) => IsOverloadOf(parsedCommand.command);
		public bool IsOverloadOf(Command command) => IsOverloadOf(command.name);
		public bool IsOverloadOf(string commandName) => string.Equals(name, commandName, StringComparison.OrdinalIgnoreCase);

		public object Execute(object[] args) => deleg.DynamicInvoke(args);

		public override bool Equals(object obj)
		{
			Command other = (Command)obj;
			if (other == null)
				return false;
			return other.deleg == deleg;
		}

		public override int GetHashCode() => deleg.GetHashCode();
	}
}