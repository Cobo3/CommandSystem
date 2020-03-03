using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem
{
	//TODO ParameterParser?
	internal class ArgumentsParser
	{
		ReflectionFinder finder;
		NotificationsHandler notificationsHandler;
		//Links a type with its Parser method
		Dictionary<Type, MethodInfo> parsers = new Dictionary<Type, MethodInfo>();

		public bool isDataLoaded { get; private set; }

		public ArgumentsParser(ReflectionFinder finder, NotificationsHandler notificationsHandler, bool allowThreading)
		{
			this.finder = finder;
			this.notificationsHandler = notificationsHandler;

			if (allowThreading)
				new Thread(FindParsers).Start();
			else
				FindParsers();
		}

		void FindParsers()
		{
			Type[] types = finder.userClassesAndStructs;
			for (int i = 0; i < types.Length; i++)
				FindParsersInType(types[i]);
			isDataLoaded = true;
			notificationsHandler.NotifyMessage($"Loaded {parsers.Count} parsers:\n{string.Join("\n", parsers.ToList().ConvertAll(x => x.Key.Namespace + "." + SignatureBuilder.TypeToString(x.Key)).ToArray())}");
		}

		void FindParsersInType(Type type)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.Length; i++)
			{
				object[] attributes = methods[i].GetCustomAttributes(typeof(ParserAttribute), false);
				if (attributes.Length > 0)
					AddParser((ParserAttribute)attributes[0], methods[i]);
			}
		}

		void AddParser(ParserAttribute attribute, MethodInfo method)
		{
			if (!parsers.ContainsKey(attribute.type))
				parsers.Add(attribute.type, method);
			else
				notificationsHandler.NotifyException(new DuplicatedParser(attribute));
		}

		public object Parse(ParsedArgument argument, Type type)
		{
			if (argument.type != null && argument.type != type)
				throw new ExplicitCastMismatch(argument.type, type);
			return Parse(argument.argument, type);
		}

		//Given a type, looks for a corresponding Parser method
		object Parse(string value, Type type)
		{
			if (type.IsEnum)
				return Enum.Parse(type, value);
			if (type.IsArray)
				return HandleArrayType(value, type);

			//TODO try get value
			if (HasParserForType(type))
				return CallParser(type, value);
			throw new NoValidParserFound(type);
		}

		object HandleArrayType(string value, Type type)
		{
			//If the type is an array, then we need to further parse the argument into an array of arguments
			//We trick that making a new command, which is the class responsible for separating the arguments...
			//TODO maybe this should be an indicator that we need a class for parsing arguments
			ParsedCommand parsedArray = new ParsedCommand("command " + value);
			Array array = (Array)Activator.CreateInstance(type, parsedArray.args.Length);
			for (int i = 0; i < parsedArray.args.Length; i++)
				array.SetValue(Parse(parsedArray.args[i], type.GetElementType()), i);
			return array;
		}

		bool HasParserForType(Type type) => parsers.ContainsKey(type);

		//TODO cache that array to not allocate
		object CallParser(Type type, string value) => parsers[type].Invoke(null, new object[] { value });
	}
}