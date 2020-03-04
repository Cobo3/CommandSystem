using System;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

namespace SickDev.CommandSystem
{
    [Serializable]
    public class CommandSystemException : Exception
    {
        public CommandSystemException(string message) : base(message) {}
        public CommandSystemException(string message, Exception innerException) : base(message, innerException) {}

        protected CommandSystemException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class InvalidCommandTypeConstructor : CommandSystemException
    {
        public Type type { get; }

        public InvalidCommandTypeConstructor(Type type) 
            : base(GetMessage(type)) 
            => this.type = type;

        static string GetMessage(Type type) => type.Name + " does not have a valid constructor. Please, review the docs on how to create new Command types";

        protected InvalidCommandTypeConstructor(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class UnsupportedCommandDeclaration : CommandSystemException
    {
        public MethodInfo method { get; }

        public UnsupportedCommandDeclaration(MethodInfo method) 
            : base(GetMessage(method)) 
            => this.method = method;

        static string GetMessage(MethodInfo method)
        {
            string message = $"The command {method.DeclaringType}.{method.Name}";
            if (!method.IsStatic)
                message += " is not static. Only static commands are supported. ";
            else if (method.IsGenericMethod || method.IsGenericMethodDefinition)
                message += " is generic, which is not yet supported. ";
            //TODO what "else" could this possibly mean?
            else
                message += " declaration is unsupported. ";
            message += "Please, review the docs on how to create new Commands";
            return message;
        }

        protected UnsupportedCommandDeclaration(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class CommandBuildingException : CommandSystemException
    {
        public Type type { get; }
        public MemberInfo member { get; }

        public CommandBuildingException(Type type, MemberInfo member, Exception innerException) :
            base(GetMessage(type, member, innerException), innerException)
        {
            this.type = type;
            this.member = member;
        }

        static string GetMessage(Type type, MemberInfo member, Exception innerException) 
            => $"There was an error creating a command for {member.MemberType} {member.Name} of type {type.Name}. Skipping command." +
                    $"\nError Message: {innerException.Message}\nStackTrace: {innerException.StackTrace}";

        protected CommandBuildingException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class DuplicatedParser : CommandSystemException
    {
        public ParserAttribute attribute { get; }

        public DuplicatedParser(ParserAttribute attribute)
            : base(GetMessage(attribute)) 
            => this.attribute = attribute;

        static string GetMessage(ParserAttribute attribute) 
            => $"More than one Parser was found for type {attribute.type}.Please, note that most common types already have a built-in Parser";

        protected DuplicatedParser(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class NoValidParserFound : CommandSystemException
    {
        public Type type { get; }

        public NoValidParserFound(Type type) 
            : base(GetMessage(type))
            => this.type = type;
        
        static string GetMessage(Type type) => $"No valid Parser method found for type {type.Name}. Please, review the docs on how to create new Parser methods";

        protected NoValidParserFound(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class InvalidArgumentFormat : CommandSystemException
    {
        public string argument { get; }
        public Type type { get; }

        public InvalidArgumentFormat(string argument, Type type)
            : base(GetMessage(argument, type))
        {
            this.argument = argument;
            this.type = type;
        }
        
        static string GetMessage(string argument, Type type) => $"Argument \"{argument}\" cannot be parsed into type {type.Name} because it is not in the correct format";

        protected InvalidArgumentFormat(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class CommandNotFound : CommandSystemException
    {
        public ParsedCommand command { get; }

        public CommandNotFound(ParsedCommand command)
            : base(GetMessage(command))
            => this.command = command;
        
        static string GetMessage (ParsedCommand command) => $"No command found with name '{command.command}'";

        protected CommandNotFound(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class MatchNotFound : CommandSystemException
    {
        public ParsedCommand command { get; }
        public Command[] overloads { get; }

        public MatchNotFound(ParsedCommand command, Command[] overloads)
            : base (GetMessage(command, overloads))
        {
            this.command = command;
            this.overloads = overloads;
        }

        static string GetMessage(ParsedCommand command, Command[] overloads)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < overloads.Length; i++)
            {
                builder.AppendLine();
                builder.Append(overloads[i].signature.raw);
            }
            return $"No match found between command '{command.raw}' and any of its overloads:{builder.ToString()}";
        }

        protected MatchNotFound(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class AmbiguousCommandCall : CommandSystemException
    {
        public ParsedCommand command { get; }
        public Command[] matches { get; }

        public AmbiguousCommandCall(ParsedCommand command, Command[] matches)
            : base (GetMessage(command, matches))
        {
            this.command = command;
            this.matches = matches;
        }

        static string GetMessage(ParsedCommand command, Command[] matches)
        { 
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < matches.Length; i++)
            {
                builder.AppendLine();
                builder.Append(matches[i].signature.raw);
            }
            return $"The command call '{command.raw}' is ambiguous between the following commands:{builder.ToString()}";
        }

        protected AmbiguousCommandCall(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class ExplicitCastNotFound : CommandSystemException
    {
        public string cast { get; }

        public ExplicitCastNotFound(string cast) 
            : base(GetMessage(cast))
            => this.cast = cast;

        static string GetMessage(string cast) => $"There is no suitable Type for the explicit cast '{cast}'";

        protected ExplicitCastNotFound(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class AmbiguousExplicitCast : CommandSystemException
    {
        public string cast { get; }
        public Type[] conflicts { get; }

        public AmbiguousExplicitCast(string cast, Type[] conflicts)
            : base (GetMessage(cast, conflicts))
        {
            this.cast = cast;
            this.conflicts = conflicts;
        }

        static string GetMessage(string cast, Type[] conflicts)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < conflicts.Length; i++)
            {
                builder.AppendLine();
                builder.Append(conflicts[i].FullName);
            }
            return $"The explicit cast '{cast}' is ambiguous between the following types:{builder.ToString()}\nPlease, refer to the Full Name of the type when casting again.";
        }

        protected AmbiguousExplicitCast(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    [Serializable]
    public class ExplicitCastMismatch : CommandSystemException
    {
        public Type castType { get; }
        public Type argumentType { get; }

        public ExplicitCastMismatch(Type castType, Type argumentType)
            : base(GetMessage(castType, argumentType))
        {
            this.castType = castType;
            this.argumentType = argumentType;
        }

        static string GetMessage(Type castType, Type argumentType) => $"The argument needs a {argumentType.Name}, whereas the cast was made to {castType.Name}";

        protected ExplicitCastMismatch(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}