using System.Linq;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class CommandExecuter {

        readonly List<CommandBase> commands;
        readonly ParsedCommand parsedCommand;
        List<CommandBase> overloads = new List<CommandBase>();
        List<Match> matches = new List<Match>();

        public bool isValidCommand { get { return overloads.Count >= 1; } }
        public bool canBeExecuted { get { return matches.Count == 1; } }
        public bool hasReturnType { get { return canBeExecuted && matches[0].command.isFunc; } }

        internal CommandExecuter(List<CommandBase> commands, ParsedCommand parsedCommand) {
            this.commands = commands;
            this.parsedCommand = parsedCommand;
            FilterOverloads();
            FilterMatches();
        }

        void FilterOverloads() {
            for(int i = 0; i < commands.Count; i++)
                if(commands[i].IsOverloadOf(parsedCommand))
                    overloads.Add(commands[i]);
        }

        void FilterMatches() {
            for (int i = 0; i < overloads.Count; i++) { 
                try {
                    if (overloads[i].signature.Matches(parsedCommand.args)) {
                        object[] arguments = overloads[i].signature.Convert(parsedCommand.args);
                        matches.Add(new Match(overloads[i], arguments));
                    }
                }
                catch (CommandSystemException exception) {
                    CommandsManager.SendException(exception);
                }
            }
        }

        public object Execute() {
            if (matches.Count > 1)
                throw new AmbiguousCommandCallException(parsedCommand.raw, matches.ConvertAll(x=>x.command).ToArray());

            if (matches.Count == 0)
                throw new CommandOverloadNotFoundException(parsedCommand);

            return matches[0].command.Execute(matches[0].parameters);
        }

        public CommandBase[] GetOverloads() {
            return overloads.ToArray();
        }

        struct Match {
            public readonly CommandBase command;
            public readonly object[] parameters;

            public Match(CommandBase command, object[] parameters) {
                this.command = command;
                this.parameters = parameters;
            }
        }
    }
}
