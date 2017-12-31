using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SickDev.CommandSystem {
    public class Signature {
        Command command;
        string _raw;

        public ParameterInfo[] parameters { get; private set; }

        public string raw {
            get {
                if (_raw == null)
                    _raw = SignatureBuilder.Build(command.method, command.name);
                return _raw;
            }
        }

        internal Signature(Command command) {
            this.command = command;
            List<ParameterInfo> parameters = new List<ParameterInfo>(command.method.GetParameters());
            parameters.RemoveAll(x => x.ParameterType == typeof(ExecutionScope));
            this.parameters = parameters.ToArray();
        }

        internal bool Matches(ParsedArgument[] args) {
            return args.Length >= parameters.Count(x => !x.IsOptional) && args.Length <= parameters.Length;
        }

        internal object[] Convert(ParsedArgument[] args, ArgumentsParser parser) {
            object[] oArgs = new object[parameters.Length];
            for (int i = 0; i < oArgs.Length; i++) {
                if (args.Length > i)
                    oArgs[i] = parser.Parse(args[i].argument, parameters[i].ParameterType);
                else
                    oArgs[i] = parameters[i].DefaultValue;
            }
            return oArgs;
        }
    }
}