using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class CommandsBuilder {
        static List<FieldInfo> fields = new List<FieldInfo>();

        string _groupPrefix;
        Type type;
        List<Command> commands = new List<Command>();

        public PropertyBuilderSettings fieldsSettings { get; set; }
        public PropertyBuilderSettings propertiesSettings { get; set; }
        public MemberBuilderSettings methodsSettings { get; set; }
        public bool addClassName { get; set; }
        public Command[] lastBuiltCommands { get { return commands.ToArray(); } }

        public string groupPrefix {
            get { return addClassName ? type.Name + "." : string.IsNullOrEmpty(_groupPrefix)?string.Empty:_groupPrefix+"."; }
            set { _groupPrefix = value; }
        }

        public CommandsBuilder(Type type) {
            this.type = type;
            fieldsSettings = new PropertyBuilderSettings();
            propertiesSettings = new PropertyBuilderSettings();
            methodsSettings = new MemberBuilderSettings();
        }

        public Command[] Build() {
            commands.Clear();
            BuildFields();
            BuildProperties();
            BuildMethods();
            return commands.ToArray();
        }

        void BuildFields() {
            FieldInfo[] fields = GetFields(fieldsSettings);
            MethodInfo setValueMethod = typeof(FieldInfo).GetMethod("SetValue", new Type[] { typeof(object), typeof(object) });
            MethodInfo getValueMethod = typeof(FieldInfo).GetMethod("GetValue", new Type[] { typeof(object) });
            ConstantExpression nullConstant = Expression.Constant(null);

            for(int i = 0; i < fields.Length; i++) {
                if(fieldsSettings.commandCreationBindings == CommandCreationBindings.None)
                    continue;

                CommandsBuilder.fields.Add(fields[i]);
                ParameterExpression valueParameter = Expression.Parameter(fields[i].FieldType, "value");
                ConstantExpression fieldConstant = Expression.Constant(fields[i]);

                if(fieldsSettings.commandCreationBindings.HasFlag(CommandCreationBindings.Getter)) {
                    try {
                        MethodCallExpression methodCall = Expression.Call(fieldConstant, getValueMethod, nullConstant);
                        UnaryExpression convertExpression = Expression.Convert(methodCall, fields[i].FieldType);
                        Type delegateType = typeof(Func<>).MakeGenericType(fields[i].FieldType);
                        LambdaExpression lambdaExpression = Expression.Lambda(delegateType, convertExpression);
                        ProcessFieldLamdaExpression(fields[i], lambdaExpression);
                    }
                    catch(Exception e) {
                        CommandsManager.SendException(new CommandBuildingException(type, fields[i], e));
                    }
                }
                if(fieldsSettings.commandCreationBindings.HasFlag(CommandCreationBindings.Setter)) {
                    if(!fields[i].IsInitOnly && !fields[i].IsLiteral) {
                        try {
                            UnaryExpression convertExpression = Expression.Convert(valueParameter, typeof(object));
                            MethodCallExpression methodCall = Expression.Call(fieldConstant, setValueMethod, nullConstant, convertExpression);
                            Type delegateType = typeof(Action<>).MakeGenericType(fields[i].FieldType);
                            LambdaExpression lambdaExpression = Expression.Lambda(delegateType, methodCall, valueParameter);
                            ProcessFieldLamdaExpression(fields[i], lambdaExpression);
                        }
                        catch(Exception e) {
                            CommandsManager.SendException(new CommandBuildingException(type, fields[i], e));
                        }
                    }
                }
            }
        }

        void ProcessFieldLamdaExpression(FieldInfo field, LambdaExpression expression) {
            Delegate deleg = expression.Compile();
            Command command = new Command(deleg, GetComposedName(field));
            commands.Add(command);
        }

        void BuildProperties() {
            PropertyInfo[] properties = GetProperties(propertiesSettings);
            for(int i = 0; i < properties.Length; i++) {
                if(propertiesSettings.commandCreationBindings.HasFlag(CommandCreationBindings.Getter) && properties[i].CanRead)
                    ProcessPropertyMethod(properties[i], properties[i].GetGetMethod(true));
                if(propertiesSettings.commandCreationBindings.HasFlag(CommandCreationBindings.Setter) && properties[i].CanWrite)
                    ProcessPropertyMethod(properties[i], properties[i].GetSetMethod(true));
            }
        }

        void ProcessPropertyMethod(PropertyInfo property, MethodInfo method) {
            try {
                if(method.IsPublic && propertiesSettings.accesModiferBindings.HasFlag(AccesModifierBindings.Public) ||
                    !method.IsPublic && propertiesSettings.accesModiferBindings.HasFlag(AccesModifierBindings.NonPublic)) {
                    commands.Add(new MethodInfoCommand(method, GetComposedName(property)));
                }
            }
            catch(Exception e) {
                CommandsManager.SendException(new CommandBuildingException(type, property, e));
            }
        }

        void BuildMethods() {
            MethodInfo[] methods = GetMethods(methodsSettings);
            for(int i = 0; i < methods.Length; i++) {
                if(methods[i].IsSpecialName)
                    continue;
                try {
                    commands.Add(new MethodInfoCommand(methods[i], GetComposedName(methods[i])));
                }
                catch(Exception e) {
                    CommandsManager.SendException(new CommandBuildingException(type, methods[i], e));
                }
            }
        }

        FieldInfo[] GetFields(MemberBuilderSettings settings) {
            return GetMembersForSettings(settings, type.GetFields);
        }

        PropertyInfo[] GetProperties(MemberBuilderSettings settings) {
            return GetMembersForSettings(settings, type.GetProperties);
        }

        MethodInfo[] GetMethods(MemberBuilderSettings settings) {
            return GetMembersForSettings(settings, type.GetMethods);
        }

        T[] GetMembersForSettings<T>(MemberBuilderSettings settings, Func<BindingFlags, T[]> callback) where T : MemberInfo {
            BindingFlags flags = GetBindingFlagsForSettings(settings);
            List<T> members = callback(flags).ToList();
            members.RemoveAll(x => settings.IsException(x));
            return members.ToArray();
        }

        BindingFlags GetBindingFlagsForSettings(MemberBuilderSettings settings) {
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

        string GetComposedName(MemberInfo member) {
            return groupPrefix + member.Name;
        }

        public class MemberBuilderSettings {
            List<string> nameExceptionsList = new List<string>();
            List<MemberInfo> memberExceptionsList = new List<MemberInfo>();

            public StaticBindings staticBindings { get; private set; }
            public AccesModifierBindings accesModiferBindings { get; set; }
            public string[] nameExceptions { get; private set; }
            public MemberInfo[] memberExceptions { get; private set; }
            public bool includeObsolete { get; set; }

            public MemberBuilderSettings() {
                staticBindings = StaticBindings.Static;
                accesModiferBindings = AccesModifierBindings.Public;
                nameExceptions = new string[0];
                memberExceptions = new MemberInfo[0];
            }

            public void AddExceptions(params string[] names) {
                nameExceptionsList.AddRange(names);
                nameExceptions = nameExceptionsList.ToArray();
            }

            public void AddExceptions(params MemberInfo[] members) {
                memberExceptionsList.AddRange(members);
                memberExceptions = memberExceptionsList.ToArray();
            }

            public bool IsException(MemberInfo member) {
                for(int i = 0; i < memberExceptions.Length; i++)
                    if(member == memberExceptions[i])
                        return true;
                for(int i = 0; i < nameExceptions.Length; i++)
                    if(member.Name == nameExceptions[i])
                        return true;

                if(includeObsolete)
                    return true;

                object[] attributes = member.GetCustomAttributes(typeof(ObsoleteAttribute), true);
                return attributes.Length > 0;
            }
        }

        public class PropertyBuilderSettings : MemberBuilderSettings {
            public CommandCreationBindings commandCreationBindings { get; private set; }

            public PropertyBuilderSettings() : base() {
                commandCreationBindings = CommandCreationBindings.Both;
            }
        }

        [Flags]
        public enum CommandCreationBindings { None = 0, Getter = 1, Setter = 2, Both = Getter | Setter }
        [Flags]
        public enum StaticBindings { None = 0, Static = 1, Instance = 2, All = Static | Instance }
        [Flags]
        public enum AccesModifierBindings { None = 0, NonPublic = 1, Public = 2, All = Public | NonPublic }
    }

    internal static class FlagsEnumExtensions {
        public static bool HasFlag(this Enum variable, Enum value) {
            return (Convert.ToUInt32(variable) & Convert.ToUInt32(value)) == Convert.ToUInt32(value);
        }
    }
}