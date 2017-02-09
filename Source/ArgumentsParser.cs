using System;
using System.Reflection;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    internal class ArgumentsParser {
        //Dictionary for linking a given type with its respective Parser method
        Dictionary<Type, MethodInfo> parsers;

        public ArgumentsParser() {
            Load();
        }

        //Finds every Parser method and adds it to the array
        void Load() {
            parsers = new Dictionary<Type, MethodInfo>();
            Type[] allTypes = ReflectionFinder.LoadUserClassesAndStructs();
            for (int i = 0; i < allTypes.Length; i++) { 
                MethodInfo[] methods = allTypes[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < methods.Length; j++) { 
                    object[] attributes = methods[j].GetCustomAttributes(typeof(ParserAttribute), false);
                    if (attributes.Length > 0) {
                        ParserAttribute parser = (ParserAttribute)attributes[0];
                        if (!parsers.ContainsKey(parser.type))
                            parsers.Add(parser.type, methods[j]);
                        else
                            CommandsManager.SendException(new DuplicatedParserException(parser));
                    }
                }
            }
        }

        bool HasParserForType(Type type) {
            return parsers.ContainsKey(type);
        }

        //Given a type, looks for a corresponding Parser method
        public object Parse(string value, Type type) {
            if (HasParserForType(type))
                return parsers[type].Invoke(null, new object[] { value });
            else
                throw new NoValidParserFoundException(type);
        }
    }
}
