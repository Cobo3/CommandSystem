using System;
using System.Linq;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class CommandsManager {
        public delegate void OnExceptionThrown(Exception exception);
        public delegate void OnMessage(string message);
        public delegate void OnCommandModified(Command command);

        List<Command> commands = new List<Command>();

        public event OnCommandModified onCommandAdded;
        public event OnCommandModified onCommandRemoved;
        public static event OnExceptionThrown onExceptionThrown;
        public static event OnMessage onMessage;

        public void Load() {
            CommandAttributeLoader loader = new CommandAttributeLoader();
            Add(loader.LoadCommands());
        }

        public void Add(Command[] commands) {
            for (int i = 0; i < commands.Length; i++)
                Add(commands[i]);
        }

        public void Add(Command command) {
            if (!commands.Contains(command) || !commands.Any(x=>x.Equals(command))) {
                commands.Add(command);
                if (onCommandAdded != null)
                    onCommandAdded(command);
            }
        }

        public void Remove(Command[] commands) {
            for(int i = 0; i < commands.Length; i++)
                Remove(commands[i]);
        }

        public void Remove(Command command) {
            if(commands.RemoveAll(x => x.Equals(command)) > 0) {
                if(onCommandRemoved != null)
                    onCommandRemoved(command);
            }
        }

        public Command[] GetCommands() {
            return commands.ToArray();
        }

        public object Execute(string text) {
            return GetCommandExecuter(text).Execute();
        }

        public CommandExecuter GetCommandExecuter(string text) {
            ParsedCommand parsedCommand = new ParsedCommand(text);
            return GetCommandExecuter(parsedCommand);
        }

        public CommandExecuter GetCommandExecuter(ParsedCommand parsedCommand) {
            return new CommandExecuter(commands, parsedCommand);            
        }

        public static void SendException(Exception exception) {
            if (onExceptionThrown != null)
                onExceptionThrown(exception);
        }

        public static void SendMessage(string message) {
            if (onMessage != null)
                onMessage(message);
        }
    }
}