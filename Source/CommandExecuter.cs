using System.Linq;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class CommandExecuter {

        readonly List<CommandBase> commands;
        readonly ParsedCommand parsedCommand;
        List<CommandBase> overloads = new List<CommandBase>();
        Dictionary<CommandBase, object[]> matches = new Dictionary<CommandBase, object[]>();

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
                        matches.Add(overloads[i], arguments);
                    }
                }
                catch (CommandSystemException exception) {
                    CommandsManager.SendException(exception);
                }
            }
        }

        public bool IsValidCommand() {
            return overloads.Count >= 1;
        }

        public bool HasReturnType() {
            return matches.Count>0 && matches.Keys.ToArray()[0].isFunc;
        }

        public object Execute() {
            if (matches.Count > 1)
                throw new AmbiguousCommandCallException(parsedCommand.raw, matches.Keys.ToArray());

            if (matches.Count == 0)
                throw new CommandOverloadNotFoundException(parsedCommand);

            var enumerator = matches.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current.Key.Execute(enumerator.Current.Value);
        }

        public CommandBase[] GetOverloads() {
            return overloads.ToArray();
        }
    }
}
