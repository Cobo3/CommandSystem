using System;
using System.Text;
using System.Reflection;

namespace SickDev.CommandSystem 
{
    public class CommandSystemException : Exception 
    {
        public CommandSystemException() { }
        public CommandSystemException(Exception innerException):base(string.Empty, innerException) { }
    }

    public class InvalidCommandTypeConstructorException : CommandSystemException 
    {
        Type type;

        public InvalidCommandTypeConstructorException(Type type) => this.type = type;

        public override string Message => type.Name + " does not have a valid constructor. Please, review the docs on how to create new Command types";
    }

    public class UnsupportedCommandDeclarationException : CommandSystemException 
    {
        MethodInfo method;

        string methodPath => method.DeclaringType + "." + method.Name;
        string header => "The command "+methodPath;
        string tail => "Please, review the docs on how to create new Commands";

        public UnsupportedCommandDeclarationException(MethodInfo method) => this.method = method;

        public override string Message 
        {
            get 
            {
                if (!method.IsStatic)
                    return header+" is not static. Only static commands are supported. " + tail;
                else if (method.IsGenericMethod || method.IsGenericMethodDefinition)
                    return header + " is generic, which is not yet supported. " + tail;
                else
                    return header + " declaration is unsupported. " +tail;
            }
        }
    }

    public class CommandBuildingException : CommandSystemException 
    {
        Type type;
        MemberInfo member;

        public CommandBuildingException(Type type, MemberInfo member, Exception innerException):
            base(innerException) 
        {
            this.type = type;
            this.member = member;
        }

        public override string Message =>
            $"There was an error creating a command for {member.MemberType} {member.Name} of type {type.Name}. Skipping command." +
                    $"\nError Message: {InnerException.Message}\nStackTrace: {InnerException.StackTrace}";
    }

    public class DuplicatedParser : CommandSystemException 
    {
        ParserAttribute parser;

        public DuplicatedParser(ParserAttribute parser) => this.parser = parser;
        public override string Message => $"More than one Parser was found for type {parser.type}.Please, note that most common types already have a built-in Parser";
    }

    public class NoValidParserFoundException : CommandSystemException
    {
        Type type;

        public NoValidParserFoundException(Type type) => this.type = type;
        public override string Message => "No valid Parser method found for type " + type.Name+". Please, review the docs on how to create new Parser methods";
    }

    public class InvalidArgumentFormatException<T> : CommandSystemException 
    {
        string argument;

        public InvalidArgumentFormatException(string argument) => this.argument = argument;
        public override string Message => "Argument \"" + argument + "\" cannot be parsed into type " + typeof(T).Name + " because it is not in the correct format";
    }

    public class CommandNotFoundException : CommandSystemException 
    {
        ParsedCommand command;

        public CommandNotFoundException(ParsedCommand command) => this.command = command;
        public override string Message => "No command found with name '" + command.command + "'";
    }

    public class MatchNotFoundException : CommandSystemException 
    {
        ParsedCommand command;
        Command[] overloads;

        public MatchNotFoundException(ParsedCommand command, Command[] overloads) 
        {
            this.command = command;
            this.overloads = overloads;
        }

        public override string Message 
        {
            get 
            {
                StringBuilder builder = new StringBuilder();
                for(int i = 0; i < overloads.Length; i++) 
                {
                    builder.AppendLine();
                    builder.Append(overloads[i].signature.raw);
                }
                return string.Format("No match found between command '{0}' and any of its overloads:{1}", command.raw, builder.ToString());
            }
        }
    }

    public class AmbiguousCommandCallException : CommandSystemException
    {
        ParsedCommand command;
        Command[] matches;

        public AmbiguousCommandCallException(ParsedCommand command, Command[] matches) 
        {
            this.command = command;
            this.matches = matches;
        }

        public override string Message 
        {
            get 
            {
                StringBuilder builder = new StringBuilder();
                for(int i = 0; i < matches.Length; i++) 
                {
                    builder.AppendLine();
                    builder.Append(matches[i].signature.raw);
                }
                return string.Format("The command call '{0}' is ambiguous between the following commands:{1}", command.raw, builder.ToString());
            }
        }
    }

    public class ExplicitCastNotFoundException : CommandSystemException 
    {
        string cast;

        public ExplicitCastNotFoundException(string cast) => this.cast = cast;
        public override string Message => string.Format("There is no suitable Type for the explicit cast '{0}'", cast);
    }

    public class AmbiguousExplicitCastException : CommandSystemException 
    {
        string cast;
        Type[] conflicts;

        public AmbiguousExplicitCastException(string cast, Type[] conflicts) 
        {
            this.cast = cast;
            this.conflicts = conflicts;
        }

        public override string Message 
        {
            get 
            {
                StringBuilder builder = new StringBuilder();
                for(int i = 0; i < conflicts.Length; i++) 
                {
                    builder.AppendLine();
                    builder.Append(conflicts[i].FullName);
                }
                return string.Format("The explicit cast '{0}' is ambiguous between the following types:{1}\nPlease, refer to the Full Name of the type when casting again.", cast, builder.ToString());
            }
        }
    }

    public class ExplicitCastMismatchException : CommandSystemException 
    {
        Type castType;
        Type argumentType;

        public ExplicitCastMismatchException(Type castType, Type argumentType) 
        {
            this.castType = castType;
            this.argumentType = argumentType;
        }

        public override string Message => string.Format("The argument needs a {0}, whereas the cast was made to {1}", argumentType.Name, castType.Name);
    }
}