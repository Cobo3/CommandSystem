using System;
using System.Reflection;

namespace SickDev.CommandSystem {
    internal class CommandAttributeVerifier {
        MethodInfo method;
        CommandAttribute attribute;

        public bool hasCommandAttribute { get { return attribute != null; } }
        public bool isDeclarationSupported { get { return !(!method.IsStatic || method.IsGenericMethod || method.IsGenericMethodDefinition); } }

        public CommandAttributeVerifier(MethodInfo method) {
            this.method = method;
            attribute = Attribute.GetCustomAttribute(method, typeof(CommandAttribute)) as CommandAttribute;
        }

        public Command ExtractCommand() {
            if (!isDeclarationSupported)
                throw new UnsupportedCommandDeclarationException(method);
            Command command = (Command)Activator.CreateInstance(typeof(MethodInfoCommand), method);
            command.alias = attribute.alias;
            command.description = attribute.description;
            command.className = attribute.className;
            command.useClassName = attribute.useClassName;
            return command;
        }
    }
}