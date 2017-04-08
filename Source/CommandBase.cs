using System;
using System.Runtime.CompilerServices;

namespace SickDev.CommandSystem {
    public abstract class CommandBase {
        internal Delegate method;
        public readonly string alias;
        public readonly string name;
        public readonly string description;
        public readonly Signature signature;
        public readonly bool isAnonymous;

        public bool isFunc { get { return method.Method.ReturnType != typeof(void); } }

        protected CommandBase(Delegate _delegate, string alias = null, string description = null) {
            method = _delegate;
            isAnonymous = method.Method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            this.description = description??string.Empty;
            this.alias = alias??string.Empty;
            name = string.IsNullOrEmpty(this.alias.Trim()) ? _delegate.Method.Name : this.alias;
            signature = new Signature(this);
        }

        public bool IsOverloadOf(ParsedCommand parsedCommand) {
            return string.Equals(name, parsedCommand.command, StringComparison.OrdinalIgnoreCase);
        }

        public object Execute(object[] args) {
            return method.DynamicInvoke(args);
        }

        public override bool Equals(object obj) {
            CommandBase other = (CommandBase)obj;
            if(other == null)
                return false;
            return other.method == method;
        }
    }
}
