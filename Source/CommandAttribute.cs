using System;

namespace SickDev.CommandSystem {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute {
        public string description { get; set; }
        public string alias { get; set; }
        
        public CommandAttribute() {}
    }
}