using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SickDev.CommandSystem {
    public class Command {
        public readonly MethodInfo method;
        public readonly string alias;
        public readonly string name;
        public readonly string description;
        public readonly Signature signature;
        public readonly bool isAnonymous;

        public bool hasReturnValue { get { return method.ReturnType != typeof(void); } }

        public Command(MethodInfo _method, string alias = null, string description = null) {
            method = _method;
            isAnonymous = method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            this.description = description??string.Empty;
            this.alias = alias??string.Empty;
            name = this.alias.Trim() == string.Empty ? _method.Name : this.alias;
            signature = new Signature(this);
        }

        public bool IsOverloadOf(ParsedCommand parsedCommand) {
            return string.Equals(name, parsedCommand.command, StringComparison.OrdinalIgnoreCase);
        }

        public object Execute(object[] args) {
            return method.Invoke(null, args);
        }

        public override bool Equals(object obj) {
            Command other = (Command)obj;
            if(other == null)
                return false;
            return other.method == method;
        }

        public override int GetHashCode() {
            return method.GetHashCode();
        }
    }
}