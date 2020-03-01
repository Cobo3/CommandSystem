using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem 
{
    internal static class SignatureBuilder 
    {
        public static readonly Dictionary<Type, string> aliases = new Dictionary<Type, string>
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(byte?), "byte?" },
            { typeof(sbyte?), "sbyte?" },
            { typeof(short?), "short?" },
            { typeof(ushort?), "ushort?" },
            { typeof(int?), "int?" },
            { typeof(uint?), "uint?" },
            { typeof(long?), "long?" },
            { typeof(ulong?), "ulong?" },
            { typeof(float?), "float?" },
            { typeof(double?), "double?" },
            { typeof(decimal?), "decimal?" },
            { typeof(bool?), "bool?" },
            { typeof(char?), "char?" },
        };

        public static string Build(MethodInfo method, string name) 
        {
            StringBuilder signature = new StringBuilder();
            if(method.ReturnType != typeof(void)) 
            {
                string returnType = TypeToString(method.ReturnType);
                signature.Append(returnType);
                signature.Append(" ");
            }
            signature.Append(name);
            List<ParameterInfo> parameters = new List<ParameterInfo>(method.GetParameters());
            if (parameters.Count > 0)
                AddParameters(signature, parameters.ToArray());
            return signature.ToString();
        }

        static void AddParameters(StringBuilder signature, ParameterInfo[] parameters) 
        {
            signature = signature.Append('(');
            for (int i = 0; i < parameters.Length; i++) 
            {
                AddParameter(signature, parameters[i]);
                if (i != parameters.Length - 1)
                    signature = signature.Append(", ");
            }
            signature = signature.Append(')');
        }

        static void AddParameter(StringBuilder signature, ParameterInfo parameter) 
        {
            signature = signature.Append(TypeToString(parameter.ParameterType));
            if(!string.IsNullOrEmpty(parameter.Name)) 
            {
                signature = signature.Append(" ");
                signature = signature.Append(parameter.Name);
            }
            if (parameter.IsOptional) 
            {
                signature = signature.Append(" = ");
                if (parameter.DefaultValue is string)
                    signature = signature.AppendFormat("\"{0}\"", parameter.DefaultValue);
                else
                    signature = signature.Append(parameter.DefaultValue);
            }
        }

        public static string TypeToString(Type type) 
        {
            StringBuilder builder = new StringBuilder();

            if (aliases.ContainsKey(type))
                builder = builder.Append(aliases[type]);
            else if (type.IsArray)
                    builder = builder.Append(TypeToString(type.GetElementType())).Append("[]");
            else if (type.IsGenericType) 
            {
                Type[] generics = type.GetGenericArguments();
                builder = builder.Append(type.Name.Substring(0, type.Name.IndexOf('`'))).Append("<");
                for (int i = 0; i < generics.Length; i++) 
                {
                    builder = builder.Append(TypeToString(generics[i]));
                    if (i != generics.Length - 1)
                        builder = builder.Append(", ");
                }
                builder = builder.Append(">");
            }
            else
                builder = builder.Append(type.Name);
            return builder.ToString();
        }
    }
}