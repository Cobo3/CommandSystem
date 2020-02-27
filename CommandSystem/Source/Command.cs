﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SickDev.CommandSystem 
{
    public class Command 
    {
        readonly Delegate deleg;
        public readonly MethodInfo method;
        public readonly Signature signature;
        public readonly bool isAnonymous;

        string _className;

        public bool useClassName { get; set; }
        public string alias { get; set; }
        public string description { get; set; }

        public string className 
        {
            get 
            {
                if(string.IsNullOrEmpty(_className))
                    return method.DeclaringType.Name;
                return _className;
            }
            set 
            {
                _className = value;
                useClassName = true;
            }
        }

        public string name 
        {
            get 
            {
                string name = string.Empty;
                if(useClassName)
                    name = className + ".";
                name += string.IsNullOrEmpty(alias) ? method.Name : alias;
                return name;
            }
        }

        public bool hasReturnValue => method.ReturnType != typeof(void);

        public Command(Delegate _deleg) 
        {
            deleg = _deleg;
            method = deleg.Method;
            isAnonymous = method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            signature = new Signature(this);
            description = string.Empty;
        }

        public bool IsOverloadOf(ParsedCommand parsedCommand) => IsOverloadOf(parsedCommand.command);
        public bool IsOverloadOf(Command command) => IsOverloadOf(command.name);
        public bool IsOverloadOf(string commandName) => string.Equals(name, commandName, StringComparison.OrdinalIgnoreCase);

        public object Execute(object[] args) => deleg.DynamicInvoke(args);

        public override bool Equals(object obj) 
        {
            Command other = (Command)obj;
            if(other == null)
                return false;
            return other.deleg == deleg;
        }

        public override int GetHashCode() => deleg.GetHashCode();
    }
}