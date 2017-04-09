using System.Linq;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class CommandsManager {
        public delegate void OnExceptionThrown(CommandSystemException exception);
        public delegate void OnMessage(string message);
        public delegate void OnCommandModified(CommandBase command);

        List<CommandBase> commands;
        Config config;

        public event OnCommandModified onCommandAdded;
        public event OnCommandModified onCommandRemoved;
        public static event OnExceptionThrown onExceptionThrown;
        public static event OnMessage onMessage;

        public CommandsManager() {
            commands = new List<CommandBase>();
            config = new Config();
        }

        public void Load() {
            CommandAttributeLoader loader = new CommandAttributeLoader(config);
            Add(loader.LoadCommands());
        }

        public void Add(CommandBase[] commands) {
            for (int i = 0; i < commands.Length; i++)
                Add(commands[i]);
        }

        public void Add(CommandBase command) {
            if (!commands.Contains(command) || !commands.Any(x=>x.Equals(command))) {
                commands.Add(command);
                if (onCommandAdded != null)
                    onCommandAdded(command);
            }
        }

        public void Remove(CommandBase[] commands) {
            for(int i = 0; i < commands.Length; i++)
                Remove(commands[i]);
        }

        public void Remove(CommandBase command) {
            if(commands.RemoveAll(x => x.Equals(command)) > 0) {
                if(onCommandRemoved != null)
                    onCommandRemoved(command);
            }
        }

        public CommandBase[] GetCommands() {
            return commands.ToArray();
        }

        public CommandExecuter GetCommandExecuter(string text) {
            ParsedCommand parsedCommand = new ParsedCommand(text);
            return GetCommandExecuter(parsedCommand);
        }

        public CommandExecuter GetCommandExecuter(ParsedCommand parsedCommand) {
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

        public void AddAssemblyWithCommands(string assemblyName) {
            config.AddAssemblyWithCommands(assemblyName);
        }
    }
}
