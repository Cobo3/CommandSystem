using System;
using System.Runtime.CompilerServices;

namespace SickDev.CommandSystem {
    public abstract class CommandBase {
        Delegate method;
        public readonly string alias;
        public readonly string name;
        public readonly string description;
        public readonly Signature signature;
        public readonly bool isAnonymous;

        public bool isFunc { get { return method.Method.ReturnType != typeof(void); } }

        protected CommandBase(Delegate _delegate, string description, string alias = null) {
            method = _delegate;
            isAnonymous = method.Method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            this.description = description;
            this.alias = alias;
            name = string.IsNullOrEmpty(alias) ? _delegate.Method.Name : alias;
            signature = new Signature(method.Method);
        }

        public bool IsOverloadOf(ParsedCommand parsedCommand) {
            return string.Equals(name, parsedCommand.command, StringComparison.OrdinalIgnoreCase);
        }

        public object Execute(object[] args) {
            return method.DynamicInvoke(args);
        }
    }
}
