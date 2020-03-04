using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem
{
	//TODO make this non static maybe?
	internal static class SignatureBuilder
	{
		public static readonly Dictionary<Type, string> aliases = new Dictionary<Type, string>
		{
			{ typeof(byte), "byte" },
			{ typeof(sbyte), "sbyte" },
			{ typeof(short), "short" },
			{ typeof(ushort), "ushort" },
			{ typeof(int), "int" },
			{ typeof(uint), "uint" },
			{ typeof(long), "long" },
			{ typeof(ulong), "ulong" },
			{ typeof(float), "float" },
			{ typeof(double), "double" },
			{ typeof(decimal), "decimal" },
			{ typeof(object), "object" },
			{ typeof(bool), "bool" },
			{ typeof(char), "char" },
			{ typeof(string), "string" },
			{ typeof(byte?), "byte?" },
			{ typeof(sbyte?), "sbyte?" },
			{ typeof(short?), "short?" },
			{ typeof(ushort?), "ushort?" },
			{ typeof(int?), "int?" },
			{ typeof(uint?), "uint?" },
			{ typeof(long?), "long?" },
			{ typeof(ulong?), "ulong?" },
			{ typeof(float?), "float?" },
			{ typeof(double?), "double?" },
			{ typeof(decimal?), "decimal?" },
			{ typeof(bool?), "bool?" },
			{ typeof(char?), "char?" },
		};

		public static string Build(MethodInfo method, string name)
		{
			StringBuilder signature = new StringBuilder();
			//TODO why not void?
			if (method.ReturnType != typeof(void))
				AddReturnType(method, signature);

			signature.Append(name);
			AddParameters(method, signature);

			return signature.ToString();
		}

		static void AddReturnType(MethodInfo method, StringBuilder signature)
		{
			string returnType = TypeToString(method.ReturnType);
			signature.Append(returnType);
			signature.Append(" ");
		}

		//TODO make this into a new class
		public static string TypeToString(Type type)
		{
			StringBuilder builder = new StringBuilder();

			if (aliases.ContainsKey(type))
				builder.Append(aliases[type]);
			else if (type.IsArray)
			{
				string elementTypeString = TypeToString(type.GetElementType());
				builder.Append(elementTypeString).Append("[]");
			}
			else if (type.IsGenericType)
			{
				Type[] generics = type.GetGenericArguments();
				//generic arguments are stringified with a '`' in .NET, which is not the conventional way of writing generic arguments in C#
				builder.Append(type.Name.Substring(0, type.Name.IndexOf('`'))).Append("<");
				for (int i = 0; i < generics.Length; i++)
				{
					builder.Append(TypeToString(generics[i]));
					if (i != generics.Length - 1)
						builder.Append(", ");
				}
				builder.Append(">");
			}
			else
				builder.Append(type.Name);
			return builder.ToString();
		}

		static void AddParameters(MethodInfo method, StringBuilder signature)
		{
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length == 0)
				return;

			signature.Append('(');
			for (int i = 0; i < parameters.Length; i++)
			{
				AddParameter(parameters[i], signature);
				if (i != parameters.Length - 1)
					signature.Append(", ");
			}
			signature.Append(')');
		}

		static void AddParameter(ParameterInfo parameter, StringBuilder signature)
		{
			signature.Append(TypeToString(parameter.ParameterType));
			AddParameterName(parameter, signature);
			AddParameterOptionalValue(parameter, signature);
		}

		static void AddParameterName(ParameterInfo parameter, StringBuilder signature)
		{
			if (string.IsNullOrEmpty(parameter.Name))
				return;

			signature.Append(" ");
			signature.Append(parameter.Name);
		}

		static void AddParameterOptionalValue(ParameterInfo parameter, StringBuilder signature)
		{
			if (!parameter.IsOptional)
				return;

			signature.Append(" = ");
			if (parameter.DefaultValue is string)
				signature.Append($"\"{parameter.DefaultValue}\"");
			else
				signature.Append(parameter.DefaultValue);
		}
	}
}