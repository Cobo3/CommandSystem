using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SickDev.CommandSystem {
    public class Command {
        internal readonly Delegate deleg;
        public MethodInfo method;
        public readonly string alias;
        public readonly string name;
        public readonly string description;
        public readonly Signature signature;
        public readonly bool isAnonymous;

        public bool hasReturnValue { get { return method.ReturnType != typeof(void); } }

        public Command(Delegate _deleg, string alias = null, string description = null) {
            deleg = _deleg;
            method = deleg.Method;
            isAnonymous = method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            this.description = description??string.Empty;
            this.alias = alias??string.Empty;
            name = this.alias.Trim() == string.Empty ? method.Name : this.alias;
            signature = new Signature(this);
        }

        public bool IsOverloadOf(ParsedCommand parsedCommand) {
            return string.Equals(name, parsedCommand.command, StringComparison.OrdinalIgnoreCase);
        }

        public object Execute(object[] args) {
            return deleg.DynamicInvoke(args);
        }

        public override bool Equals(object obj) {
            Command other = (Command)obj;
            if(other == null)
                return false;
            return other.deleg == deleg;
        }

        public override int GetHashCode() {
            return deleg.GetHashCode();
        }
    }
}