using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SickDev.CommandSystem
{
	public class CommandsBuilder
	{
		Type type;
		NotificationsHandler notificationsHandler;
		List<Command> commands = new List<Command>();

		public PropertyBuilderSettings fieldsSettings { get; set; }
		public PropertyBuilderSettings propertiesSettings { get; set; }
		public MemberBuilderSettings methodsSettings { get; set; }
		public bool useClassName { get; set; }
		public string className { get; set; }

		public Command[] lastBuiltCommands => commands.ToArray();

		public CommandsBuilder(Type type, NotificationsHandler notificationsHandler)
		{
			this.type = type;
			this.notificationsHandler = notificationsHandler;
			className = type.Name;
			fieldsSettings = new PropertyBuilderSettings();
			propertiesSettings = new PropertyBuilderSettings();
			methodsSettings = new MemberBuilderSettings();
		}

		public Command[] Build()
		{
			commands.Clear();
			BuildFields();
			BuildProperties();
			BuildMethods();
			return commands.ToArray();
		}

		FieldInfo[] GetFields() => GetMembersForSettings(fieldsSettings, type.GetFields);
		PropertyInfo[] GetProperties() => GetMembersForSettings(propertiesSettings, type.GetProperties);
		MethodInfo[] GetMethods() => GetMembersForSettings(methodsSettings, type.GetMethods);

		T[] GetMembersForSettings<T>(MemberBuilderSettings settings, Func<BindingFlags, T[]> callback) where T : MemberInfo
		{
			BindingFlags flags = GetBindingFlagsForSettings(settings);
			List<T> members = callback(flags).ToList();
			members.RemoveAll(x => settings.IsException(x));
			return members.ToArray();
		}

		BindingFlags GetBindingFlagsForSettings(MemberBuilderSettings settings)
		{
			BindingFlags flags = BindingFlags.DeclaredOnly;

			if (settings.includePublic)
				flags |= BindingFlags.Public;
			if (settings.includeNonPublic)
				flags |= BindingFlags.NonPublic;

			if (settings.staticBindings.HasFlag(StaticBindings.Instance))
				flags |= BindingFlags.Instance;
			if (settings.staticBindings.HasFlag(StaticBindings.Static))
				flags |= BindingFlags.Static;

			return flags;
		}

		//Foreach field, make a command to get its value and another to set it
		void BuildFields()
		{
			if (fieldsSettings.propertyAccessorBindings == PropertyAccessorBindings.None)
				return;

			FieldInfo[] fields = GetFields();
			MethodInfo getValueMethod = typeof(FieldInfo).GetMethod(nameof(FieldInfo.GetValue), new Type[] { typeof(object) });
			MethodInfo setValueMethod = typeof(FieldInfo).GetMethod(nameof(FieldInfo.SetValue), new Type[] { typeof(object), typeof(object) });
			ConstantExpression nullConstant = Expression.Constant(null);

			for (int i = 0; i < fields.Length; i++)
			{
				ConstantExpression fieldConstant = Expression.Constant(fields[i]);
				MakeFieldGetter(fields[i], fieldConstant, nullConstant, getValueMethod);
				MakeFieldSetter(fields[i], fieldConstant, nullConstant, setValueMethod);
			}
		}

		void MakeFieldGetter(FieldInfo field, ConstantExpression fieldConstant, ConstantExpression nullConstant, MethodInfo getValueMethod)
		{
			if (!fieldsSettings.includeGetter)
				return;

			try
			{
				/* The generated code would look something like this, just without any of the assignments
                    * Func<FieldType> lambda = ()=>
                    * {
                    *      object returnValue = fields[i].GetValue(null);
                    *      FieldType castedReturnValue = (FieldType) returnValue;
                    *      return castedReturnValue;
                    * }
                    */
				MethodCallExpression methodCall = Expression.Call(fieldConstant, getValueMethod, nullConstant);
				UnaryExpression convertExpression = Expression.Convert(methodCall, field.FieldType);
				Type delegateType = typeof(Func<>).MakeGenericType(field.FieldType);
				LambdaExpression lambdaExpression = Expression.Lambda(delegateType, convertExpression);
				MakeLambdaIntoACommand(field, lambdaExpression);
			}
			//TODO be more specific here
			catch (Exception e)
			{
				notificationsHandler.NotifyException(new CommandBuildingException(type, field, e));
			}
		}

		void MakeFieldSetter(FieldInfo field, ConstantExpression fieldConstant, ConstantExpression nullConstant, MethodInfo setValueMethod)
		{
			if (field.IsInitOnly || field.IsLiteral || !fieldsSettings.includeSetter)
				return;

			try
			{
				/* The generated code would look something like this, just without any of the assignments
                    * Action<FieldType> lambda = (FieldType value) =>
                    * {
                    *      object valueAsObject = (object) value;
                    *      fields[i].SetValue(null, valueAsObject);
                    * }
                    */
				ParameterExpression valueParameter = Expression.Parameter(field.FieldType, "value");
				//TODO is this cast really necessary?
				UnaryExpression convertExpression = Expression.Convert(valueParameter, typeof(object));
				MethodCallExpression methodCall = Expression.Call(fieldConstant, setValueMethod, nullConstant, convertExpression);
				Type delegateType = typeof(Action<>).MakeGenericType(field.FieldType);
				LambdaExpression lambdaExpression = Expression.Lambda(delegateType, methodCall, valueParameter);
				MakeLambdaIntoACommand(field, lambdaExpression);
			}
			catch (Exception e)
			{
				notificationsHandler.NotifyException(new CommandBuildingException(type, field, e));
			}
		}

		void MakeLambdaIntoACommand(FieldInfo field, LambdaExpression expression)
		{
			Delegate deleg = expression.Compile();
			Command command = new Command(deleg)
			{
				alias = field.Name,
				className = className,
				useClassName = useClassName
			};
			commands.Add(command);
		}

		//Foreach field, make a command out of its getter and another out of its setter
		void BuildProperties()
		{
			PropertyInfo[] properties = GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].CanRead && propertiesSettings.includeGetter)
					ProcessPropertyMethod(properties[i].GetGetMethod(true));
				if (properties[i].CanWrite && propertiesSettings.includeSetter)
					ProcessPropertyMethod(properties[i].GetSetMethod(true));
			}
		}

		void ProcessPropertyMethod(MethodInfo method)
		{
			if (method.IsPublic && !propertiesSettings.includePublic ||
				!method.IsPublic && !propertiesSettings.includeNonPublic)
				return;

			commands.Add(new MethodInfoCommand(method)
			{
				className = className,
				useClassName = useClassName
			});
		}

		void BuildMethods()
		{
			MethodInfo[] methods = GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				if (methods[i].IsSpecialName)
					continue;

				commands.Add(new MethodInfoCommand(methods[i])
				{
					className = className,
					useClassName = useClassName
				});
			}
		}

		public class MemberBuilderSettings
		{
			List<string> nameExceptionsList = new List<string>();
			List<MemberInfo> memberExceptionsList = new List<MemberInfo>();

			//Currently only Static members are supported
			public StaticBindings staticBindings { get; private set; }
			public AccessModifierBindings accessModiferBindings { get; set; }
			public string[] nameExceptions { get; private set; }
			public MemberInfo[] memberExceptions { get; private set; }
			public bool includeObsolete { get; set; }

			public bool includePublic => accessModiferBindings.HasFlag(AccessModifierBindings.Public);
			public bool includeNonPublic => accessModiferBindings.HasFlag(AccessModifierBindings.NonPublic);

			public MemberBuilderSettings()
			{
				staticBindings = StaticBindings.Static;
				accessModiferBindings = AccessModifierBindings.Public;
				nameExceptions = new string[0];
				memberExceptions = new MemberInfo[0];
			}

			public void AddExceptions(params string[] names)
			{
				nameExceptionsList.AddRange(names);
				nameExceptions = nameExceptionsList.ToArray();
			}

			public void AddExceptions(params MemberInfo[] members)
			{
				memberExceptionsList.AddRange(members);
				memberExceptions = memberExceptionsList.ToArray();
			}

			public bool IsException(MemberInfo member)
			{
				//Explicit member exception
				for (int i = 0; i < memberExceptions.Length; i++)
					if (member == memberExceptions[i])
						return true;

				//Explicit name exception
				for (int i = 0; i < nameExceptions.Length; i++)
					if (member.Name == nameExceptions[i])
						return true;

				//If we include obsolete members, then those members are never an excepion, regardless of whether they are obsolete or not
				if (includeObsolete)
					return false;

				//Otherwise, if they are not included, then members marked as obsolete are indeed exceptions
				object[] attributes = member.GetCustomAttributes(typeof(ObsoleteAttribute), true);
				return attributes.Length > 0;
			}
		}

		public class PropertyBuilderSettings : MemberBuilderSettings
		{
			public PropertyAccessorBindings propertyAccessorBindings { get; set; }

			public bool includeGetter => propertyAccessorBindings.HasFlag(PropertyAccessorBindings.Getter);
			public bool includeSetter => propertyAccessorBindings.HasFlag(PropertyAccessorBindings.Setter);

			public PropertyBuilderSettings() : base() =>
				propertyAccessorBindings = PropertyAccessorBindings.Both;
		}

		[Flags]
		public enum PropertyAccessorBindings { None = 0, Getter = 1, Setter = 2, Both = Getter | Setter }
		[Flags]
		public enum StaticBindings { None = 0, Static = 1, Instance = 2, All = Static | Instance }
		[Flags]
		public enum AccessModifierBindings { None = 0, NonPublic = 1, Public = 2, All = Public | NonPublic }
	}

	internal static class FlagsEnumExtensions
	{
		public static bool HasFlag(this Enum variable, Enum value) => (Convert.ToUInt32(variable) & Convert.ToUInt32(value)) == Convert.ToUInt32(value);
	}
}