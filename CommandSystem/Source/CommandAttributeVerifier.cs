using System;
using System.Reflection;

namespace SickDev.CommandSystem {
    internal class CommandAttributeVerifier {
        MethodInfo method;
        CommandAttribute attribute;
        CommandSystemException exception;

        public bool hasCommandAttribute { get { return attribute != null; } }
        bool isDeclarationSupported { get { return !(!method.IsStatic || method.IsGenericMethod || method.IsGenericMethodDefinition); } }

        public CommandAttributeVerifier(MethodInfo method) {
            this.method = method;
            attribute = Attribute.GetCustomAttribute(method, typeof(CommandAttribute)) as CommandAttribute;
        }

        public Command ExtractCommand() {
            if (!isDeclarationSupported)
                throw new UnsupportedCommandDeclarationException(method);
            return (Command)Activator.CreateInstance(typeof(Command), method, attribute.alias, attribute.description);
        }
    }
}