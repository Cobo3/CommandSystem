using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class CommandExecuter {

        readonly List<Command> commands;
        readonly ParsedCommand parsedCommand;
        List<Command> overloads = new List<Command>();
        List<Match> matches = new List<Match>();

        public bool isValidCommand { get { return overloads.Count >= 1; } }
        public bool canBeExecuted { get { return matches.Count == 1; } }
        public bool hasReturnValue { get { return canBeExecuted && matches[0].command.hasReturnValue; } }

        internal CommandExecuter(List<Command> commands, ParsedCommand parsedCommand) {
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
            if(!isValidCommand)
                throw new CommandNotFoundException(parsedCommand);

            if (matches.Count > 1)
                throw new AmbiguousCommandCallException(parsedCommand.raw, matches.ConvertAll(x=>x.command).ToArray());

            if (matches.Count == 0)
                throw new OverloadNotFoundException(parsedCommand);

            return matches[0].command.Execute(matches[0].parameters);
        }

        public Command[] GetOverloads() {
            return overloads.ToArray();
        }

        struct Match {
            public readonly Command command;
            public readonly object[] parameters;

            public Match(Command command, object[] parameters) {
                this.command = command;
                this.parameters = parameters;
            }
        }
    }
}
