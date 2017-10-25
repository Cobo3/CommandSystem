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
            RemoveInternal(x => command.Equals(x));
        }

        public void RemoveOverloads(Command[] commands) {
            for(int i = 0; i < commands.Length; i++)
                RemoveOverloads(commands[i]);
        }

        public void RemoveOverloads(Command command) {
            RemoveInternal(x => command.IsOverloadOf(x));
        }

        public bool IsCommandAdded(Command command) {
            return commands.Any(x => command.Equals(x));
        }

        public bool IsCommandOverloadAdded(Command command) {
            return commands.Any(x => command.IsOverloadOf(x));
        }

        void RemoveInternal(Predicate<Command> predicate) {
            for(int i = commands.Count - 1; i >= 0; i--) {
                if(predicate(commands[i])) { 
                    if(onCommandRemoved != null)
                        onCommandRemoved(commands[i]);
                }
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