using System;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class CommandsManager {
        public delegate void OnExceptionThrown(CommandSystemException exception);
        public delegate void OnMessage(string message);
        public delegate void OnCommandAdded(CommandBase command);

        List<CommandBase> commands;

        public event OnCommandAdded onCommandAdded;
        public static event OnExceptionThrown onExceptionThrown;
        public static event OnMessage onMessage;

        public CommandsManager() {
            commands = new List<CommandBase>();
        }

        internal void Load() {
            CommandAttributeLoader loader = new CommandAttributeLoader();
            Add(loader.LoadCommands());
        }

        public void Add(CommandBase[] commands) {
            for (int i = 0; i < commands.Length; i++)
                Add(commands[i]);
        }

        public void Add(CommandBase command) {
            if (!commands.Contains(command)) {
                commands.Add(command);
                if (onCommandAdded != null)
                    onCommandAdded(command);
            }
        }

        public CommandBase[] GetCommands() {
            return commands.ToArray();
        }

        internal CommandExecuter GetCommandExecuter(string text) {
            ParsedCommand parsedCommand = new ParsedCommand(text);
            return GetCommandExecuter(parsedCommand);
        }

        internal CommandExecuter GetCommandExecuter(ParsedCommand parsedCommand) {
            return new CommandExecuter(commands, parsedCommand);            
        }

        internal static void SendException(CommandSystemException exception) {
            if (onExceptionThrown != null)
                onExceptionThrown(exception);
        }

        internal static void SendMessage(string message) {
            if (onMessage != null)
                onMessage(message);
        }
    }
}
