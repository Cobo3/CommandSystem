using System;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    internal class CommandAttributeLoader {
        List<Command> commands = new List<Command>();
        Type[] types;
        CommandTypeInfo[] commandTypes;

        public CommandAttributeLoader() {
            types = ReflectionFinder.LoadUserClassesAndStructs();
            commandTypes = FilterCommandTypes(types);
        }
        
        //Get all types derived from CommandBase and create a CommandTypeInfo for each one
        static CommandTypeInfo[] FilterCommandTypes(Type[] types) {
            List<CommandTypeInfo> commandTypes = new List<CommandTypeInfo>();
            for (int i = 0; i < types.Length; i++) {
                if (types[i].IsSubclassOf(typeof(Command)))
                    commandTypes.Add(new CommandTypeInfo(types[i]));
            }
            return commandTypes.ToArray();
        }

        public Command[] LoadCommands() {
            for (int i = 0; i < types.Length; i++)
                commands.AddRange(LoadCommandsInType(types[i]));
            return commands.ToArray();
        }

        Command[] LoadCommandsInType(Type type) {
            List<Command> commands = new List<Command>();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < methods.Length; i++) {
                try {
                    CommandAttributeVerifier verifier = new CommandAttributeVerifier(methods[i]);
                    if (verifier.hasCommandAttribute) {
                        Command command = verifier.ExtractCommand(commandTypes);
                        if (command != null)
                            commands.Add(command);
                    }
                }
                catch (CommandSystemException exception) {
                    CommandsManager.SendException(exception);
                }
            }
            return commands.ToArray();
        }
    }
}