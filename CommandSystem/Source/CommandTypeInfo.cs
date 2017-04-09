using System;
using System.Reflection;

namespace SickDev.CommandSystem {
    //Holds information about a CommandType
    internal class CommandTypeInfo {
        public Type type { get; private set; }
        public bool isGeneric { get; private set; }
        public ConstructorInfo constructor { get; private set; }
        public ParameterInfo firstParameter { get; private set; }
        public bool isFunc { get; private set; }
        public int parametersLength { get; private set; }

        public CommandTypeInfo(Type type) {
            try {
                this.type = type;
                isGeneric = type.IsGenericType && type.IsGenericTypeDefinition;
                SetConstructor();
                SetExtraInfo();
            }
            catch (CommandSystemException exception) {
                CommandsManager.SendException(exception);
            }
        }

        void SetConstructor() {
            bool found = false;
            ConstructorInfo[] constructors = type.GetConstructors();
            for (int i = 0; i < constructors.Length; i++) {
                if (!constructors[i].IsPublic)
                    continue;
                ParameterInfo[] parameters = constructors[i].GetParameters();
                if (parameters.Length>=3 && parameters[0].ParameterType.IsSubclassOf(typeof(Delegate)) && 
                    parameters[1].ParameterType == typeof(string) && 
                    parameters[2].ParameterType == typeof(string)) {
                    constructor = constructors[i];
                    firstParameter = parameters[0];
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new InvalidCommandTypeConstructorException(type);
        }

        void SetExtraInfo() {
            MethodInfo invokeMethod = firstParameter.ParameterType.GetMethod("Invoke");
            isFunc = invokeMethod.ReturnType != typeof(void);
            parametersLength = invokeMethod.GetParameters().Length;
        }

        public CommandTypeInfo MakeGeneric(Type[] paramTypes) {
            Type type = this.type.MakeGenericType(paramTypes);
            return new CommandTypeInfo(type);
        }
    }
}