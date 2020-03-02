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

        void BuildFields() 
        {
            FieldInfo[] fields = GetFields(fieldsSettings);
            MethodInfo setValueMethod = typeof(FieldInfo).GetMethod("SetValue", new Type[] { typeof(object), typeof(object) });
            MethodInfo getValueMethod = typeof(FieldInfo).GetMethod("GetValue", new Type[] { typeof(object) });
            ConstantExpression nullConstant = Expression.Constant(null);

            for(int i = 0; i < fields.Length; i++) 
            {
                if(fieldsSettings.accessorCreationBindings == AccessorCreationBindings.None)
                    continue;

                ParameterExpression valueParameter = Expression.Parameter(fields[i].FieldType, "value");
                ConstantExpression fieldConstant = Expression.Constant(fields[i]);

                if(fieldsSettings.accessorCreationBindings.HasFlag(AccessorCreationBindings.Getter)) 
                {
                    try 
                    {
                        MethodCallExpression methodCall = Expression.Call(fieldConstant, getValueMethod, nullConstant);
                        UnaryExpression convertExpression = Expression.Convert(methodCall, fields[i].FieldType);
                        Type delegateType = typeof(Func<>).MakeGenericType(fields[i].FieldType);
                        LambdaExpression lambdaExpression = Expression.Lambda(delegateType, convertExpression);
                        ProcessFieldLamdaExpression(fields[i], lambdaExpression);
                    }
                    catch(Exception e) 
                    {
                        notificationsHandler.NotifyException(new CommandBuildingException(type, fields[i], e));
                    }
                }
                if(fieldsSettings.accessorCreationBindings.HasFlag(AccessorCreationBindings.Setter)) 
                {
                    if(!fields[i].IsInitOnly && !fields[i].IsLiteral) 
                    {
                        try 
                        {
                            UnaryExpression convertExpression = Expression.Convert(valueParameter, typeof(object));
                            MethodCallExpression methodCall = Expression.Call(fieldConstant, setValueMethod, nullConstant, convertExpression);
                            Type delegateType = typeof(Action<>).MakeGenericType(fields[i].FieldType);
                            LambdaExpression lambdaExpression = Expression.Lambda(delegateType, methodCall, valueParameter);
                            ProcessFieldLamdaExpression(fields[i], lambdaExpression);
                        }
                        catch(Exception e) 
                        {
                            notificationsHandler.NotifyException(new CommandBuildingException(type, fields[i], e));
                        }
                    }
                }
            }
        }

        void ProcessFieldLamdaExpression(FieldInfo field, LambdaExpression expression) 
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

        void BuildProperties() 
        {
            PropertyInfo[] properties = GetProperties(propertiesSettings);
            for(int i = 0; i < properties.Length; i++) 
            {
                if(propertiesSettings.accessorCreationBindings.HasFlag(AccessorCreationBindings.Getter) && properties[i].CanRead)
                    ProcessPropertyMethod(properties[i], properties[i].GetGetMethod(true));
                if(propertiesSettings.accessorCreationBindings.HasFlag(AccessorCreationBindings.Setter) && properties[i].CanWrite)
                    ProcessPropertyMethod(properties[i], properties[i].GetSetMethod(true));
            }
        }

        void ProcessPropertyMethod(PropertyInfo property, MethodInfo method) 
        {
            try 
            {
                if(method.IsPublic && propertiesSettings.accesModiferBindings.HasFlag(AccesModifierBindings.Public) ||
                    !method.IsPublic && propertiesSettings.accesModiferBindings.HasFlag(AccesModifierBindings.NonPublic)) 
                {
                    commands.Add(new MethodInfoCommand(method) 
                    {
                        className = className,
                        useClassName = useClassName
                    });
                }
            }
            catch(Exception e) 
            {
                notificationsHandler.NotifyException(new CommandBuildingException(type, property, e));
            }
        }

        void BuildMethods() 
        {
            MethodInfo[] methods = GetMethods(methodsSettings);
            for(int i = 0; i < methods.Length; i++) 
            {
                if(methods[i].IsSpecialName)
                    continue;
                try 
                {
                    commands.Add(new MethodInfoCommand(methods[i]) 
                    {
                        className = className,
                        useClassName = useClassName
                    });
                }
                catch(Exception e) 
                {
                    notificationsHandler.NotifyException(new CommandBuildingException(type, methods[i], e));
                }
            }
        }

        FieldInfo[] GetFields(MemberBuilderSettings settings) => GetMembersForSettings(settings, type.GetFields);
        PropertyInfo[] GetProperties(MemberBuilderSettings settings) => GetMembersForSettings(settings, type.GetProperties);
        MethodInfo[] GetMethods(MemberBuilderSettings settings) => GetMembersForSettings(settings, type.GetMethods);

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

            if(settings.accesModiferBindings.HasFlag(AccesModifierBindings.Public))
                flags |= BindingFlags.Public;
            if(settings.accesModiferBindings.HasFlag(AccesModifierBindings.NonPublic))
                flags |= BindingFlags.NonPublic;

            if(settings.staticBindings.HasFlag(StaticBindings.Instance))
                flags |= BindingFlags.Instance;
            if(settings.staticBindings.HasFlag(StaticBindings.Static))
                flags |= BindingFlags.Static;

            return flags;
        }

        public class MemberBuilderSettings 
        {
            List<string> nameExceptionsList = new List<string>();
            List<MemberInfo> memberExceptionsList = new List<MemberInfo>();

            public StaticBindings staticBindings { get; private set; }
            public AccesModifierBindings accesModiferBindings { get; set; }
            public string[] nameExceptions { get; private set; }
            public MemberInfo[] memberExceptions { get; private set; }
            public bool includeObsolete { get; set; }

            public MemberBuilderSettings() 
            {
                staticBindings = StaticBindings.Static;
                accesModiferBindings = AccesModifierBindings.Public;
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
                for(int i = 0; i < memberExceptions.Length; i++)
                    if(member == memberExceptions[i])
                        return true;
                for(int i = 0; i < nameExceptions.Length; i++)
                    if(member.Name == nameExceptions[i])
                        return true;

                //If we include obsolete members, then those members are never an excepion, regardless of whether they are obsolete or not
                if(includeObsolete)
                    return false;

                //Otherwise, if they are not included, then members marked as obsolete are indeed exceptions
                object[] attributes = member.GetCustomAttributes(typeof(ObsoleteAttribute), true);
                return attributes.Length > 0;
            }
        }

        public class PropertyBuilderSettings : MemberBuilderSettings 
        {
            public AccessorCreationBindings accessorCreationBindings { get; set; }

            public PropertyBuilderSettings() : base() =>
                accessorCreationBindings = AccessorCreationBindings.Both;
        }

        [Flags]
        public enum AccessorCreationBindings { None = 0, Getter = 1, Setter = 2, Both = Getter | Setter }
        [Flags]
        public enum StaticBindings { None = 0, Static = 1, Instance = 2, All = Static | Instance }
        [Flags]
        public enum AccesModifierBindings { None = 0, NonPublic = 1, Public = 2, All = Public | NonPublic }
    }

    internal static class FlagsEnumExtensions 
    {
        public static bool HasFlag(this Enum variable, Enum value) => (Convert.ToUInt32(variable) & Convert.ToUInt32(value)) == Convert.ToUInt32(value);
    }
}