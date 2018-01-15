using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class CommandsManager {
        public delegate void OnExceptionThrown(Exception exception);
        public delegate void OnMessage(string message);
        public delegate void OnCommandModified(Command command);

        object block = new object();
        List<Command> commands = new List<Command>();

        readonly Configuration configuration;
        readonly ReflectionFinder finder;
        readonly ArgumentsParser parser;
        readonly CommandAttributeLoader loader;

        public event OnCommandModified onCommandAdded;
        public event OnCommandModified onCommandRemoved;
        public static event OnExceptionThrown onExceptionThrown;
        public static event OnMessage onMessage;

        public bool allDataLoaded { get { return parser.dataLoaded; } }

        public CommandsManager(Configuration configuration) {
            this.configuration = configuration;
            finder = new ReflectionFinder(configuration);
            parser = new ArgumentsParser(finder);
            loader = new CommandAttributeLoader(finder);
        }

        public void LoadCommands() {
            Thread thread = new Thread(LoadThreaded);
            thread.Start();
        }

        void LoadThreaded() {
            Add(loader.LoadCommands());
        }

        public void Add(Command[] commands) {
            for (int i = 0; i < commands.Length; i++)
                Add(commands[i]);
        }

        public void Add(Command command) {
            lock(block) {
                if (!commands.Contains(command) || !commands.Any(x=>x.Equals(command))) {
                    commands.Add(command);
                    if (onCommandAdded != null)
                        onCommandAdded(command);
                }
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
                    Command removedCommand = commands[i];
                    commands.RemoveAt(i);
                    if(onCommandRemoved != null)
                        onCommandRemoved(removedCommand);
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
            return new CommandExecuter(commands, parsedCommand, parser);            
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