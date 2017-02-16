using System.Linq;
using System.Reflection;

namespace SickDev.CommandSystem {
    public class Signature {
        CommandBase command;
        string _raw;

        public ParameterInfo[] parameters { get; private set; }

        public string raw {
            get {
                if (_raw == null)
                    _raw = SignatureBuilder.Build(command.method.Method, command.name);
                return _raw;
            }
        }

        static ArgumentsParser _parser;
        static ArgumentsParser parser {
            get {
                if (_parser == null)
                    _parser = new ArgumentsParser();
                return _parser;
            }
        }

        internal Signature(CommandBase command) {
            this.command = command;
            parameters = command.method.Method.GetParameters();
        }

        internal bool Matches(string[] args) {
            return args.Length == parameters.Length || args.Length == parameters.Count(x => !x.IsOptional);
        }

        internal object[] Convert(string[] args) {
            object[] oArgs = GetArguments(args);
            return oArgs;
        }

        object[] GetArguments(string[] args) {
            object[] oArgs = new object[parameters.Length];
            for (int i = 0; i < oArgs.Length; i++) {
                if (args.Length > i)
                    oArgs[i] = parser.Parse(args[i], parameters[i].ParameterType);
                else
                    oArgs[i] = parameters[i].DefaultValue;
            }
            return oArgs;
        }
    }
}