using System;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    internal class CommandAttributeLoader {
        List<Command> commands = new List<Command>();
        Type[] types;

        public CommandAttributeLoader() {
            types = ReflectionFinder.LoadClassesAndStructs();
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
                        Command command = verifier.ExtractCommand();
                        commands.Add(command);
                    }
                }
                catch (Exception exception) {
                    CommandsManager.SendException(exception);
                }
            }
            return commands.ToArray();
        }
    }
}