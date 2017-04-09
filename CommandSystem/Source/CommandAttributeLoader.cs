using System;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    internal class CommandAttributeLoader {
        List<CommandBase> commands = new List<CommandBase>();
        Type[] types;
        CommandTypeInfo[] commandTypes;

        public CommandAttributeLoader(Config config) {
            types = ReflectionFinder.LoadUserClassesAndStructs(config.assembliesWithCommands);
            commandTypes = FilterCommandTypes(types);
        }
        
        //Get all types derived from CommandBase and create a CommandTypeInfo for each one
        static CommandTypeInfo[] FilterCommandTypes(Type[] types) {
            List<CommandTypeInfo> commandTypes = new List<CommandTypeInfo>();
            for (int i = 0; i < types.Length; i++) {
                if (types[i].IsSubclassOf(typeof(CommandBase)))
                    commandTypes.Add(new CommandTypeInfo(types[i]));
            }
            return commandTypes.ToArray();
        }

        public CommandBase[] LoadCommands() {
            for (int i = 0; i < types.Length; i++)
                commands.AddRange(LoadCommandsInType(types[i]));
            return commands.ToArray();
        }

        CommandBase[] LoadCommandsInType(Type type) {
            List<CommandBase> commands = new List<CommandBase>();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < methods.Length; i++) {
                try {
                    CommandAttributeVerifier verifier = new CommandAttributeVerifier(methods[i]);
                    if (verifier.hasCommandAttribute) {
                        CommandBase command = verifier.ExtractCommand(commandTypes);
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