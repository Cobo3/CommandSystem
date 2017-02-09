using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SickDev.CommandSystem {
    public abstract class CommandBase {
        Delegate method;
        public readonly string[] aliases;
        public readonly string name;
        public readonly string description;
        public readonly Signature signature;
        public readonly bool isAnonymous;

        public bool isFunc { get { return method.Method.ReturnType != typeof(void); } }

        protected CommandBase(Delegate _delegate, string description, string[] aliases = null) {
            method = _delegate;
            isAnonymous = method.Method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            this.description = description;
            this.aliases = new List<string>(aliases??new string[0] {}).ToArray();
            name = isAnonymous?(aliases==null || aliases.Length ==0)?_delegate.Method.Name:aliases[0]:_delegate.Method.Name;
            signature = new Signature(method.Method);
        }

        public bool IsOverloadOf(ParsedCommand parsedCommand) {
            bool result = string.Equals(name, parsedCommand.command, StringComparison.OrdinalIgnoreCase);
            int nextAlias = 0;
            while (!result && nextAlias<aliases.Length)
                result |= string.Equals(aliases[nextAlias++], parsedCommand.command, StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public object Execute(object[] args) {
            return method.DynamicInvoke(args);
        }
    }
}
