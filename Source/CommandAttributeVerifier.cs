using System;
using System.Linq;
using System.Reflection;

namespace SickDev.CommandSystem {
    internal class CommandAttributeVerifier {
        MethodInfo method;
        CommandAttribute attribute;
        CommandTypeInfo commandType;

        public bool hasCommandAttribute { get { return attribute != null; } }

        public CommandAttributeVerifier(MethodInfo method) {
            this.method = method;
            attribute = Attribute.GetCustomAttribute(method, typeof(CommandAttribute)) as CommandAttribute;
        }

        public CommandBase ExtractCommand(CommandTypeInfo[] commandTypes) {
            CommandBase command = null;
            if (IsDeclarationSupported()) {
                CheckCommandTypeMatch(commandTypes);
                if (commandType == null)
                    throw new NoSuitableCommandFoundException(method);
                command = (CommandBase)Activator.CreateInstance(commandType.type, Delegate.CreateDelegate(
                    commandType.firstParameter.ParameterType, method), attribute.description, attribute.aliases);
            }
            else
                throw new UnsupportedCommandDeclarationException(method);
            return command;
        }

        bool IsDeclarationSupported() {
            return !(!method.IsStatic || method.IsGenericMethod || method.IsGenericMethodDefinition);
        }

        void CheckCommandTypeMatch(CommandTypeInfo[] commandTypes) {
            ParameterInfo[] parameters = method.GetParameters();
            Type[] paramTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                paramTypes[i] = parameters[i].ParameterType;

            for (int i = 0; i < commandTypes.Length; i++) {
                if (parameters.Length == commandTypes[i].parametersLength) {
                    if (BothAreAction(method, commandTypes[i])) {
                        if (commandTypes[i].isGeneric)
                            commandType = commandTypes[i].MakeGeneric(paramTypes);
                        else
                            commandType = commandTypes[i];
                        break;
                    }
                    else if (BothAreFunc(method, commandTypes[i])) {
                        if (commandTypes[i].isGeneric)
                            commandType = commandTypes[i].MakeGeneric(paramTypes.Concat(new Type[] { method.ReturnType }).ToArray());
                        else
                            commandType = commandTypes[i];
                        break;
                    }
                }
            }
        }

        static bool BothAreAction(MethodInfo method, CommandTypeInfo commandType) {
            return method.ReturnType == typeof(void) && !commandType.isFunc;
        }

        static bool BothAreFunc(MethodInfo method, CommandTypeInfo commandType) {
            return method.ReturnType != typeof(void) && commandType.isFunc;
        }
    }
}