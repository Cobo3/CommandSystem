using System;

namespace SickDev.CommandSystem 
{
    //Atribbute used to identify methods as Parsers.
    //A Parser method gets a string as only argument and returns some other value
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ParserAttribute : Attribute 
    {
        public readonly Type type;
        public ParserAttribute(Type type) => this.type = type;
    }
}