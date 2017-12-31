using System;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class ParsedArgument {
        public string raw { get; private set; }
        public string argument { get; private set; }
        public Type type { get; private set; }

        public bool isTypeSpecified { get { return type != null; } }

        public ParsedArgument(string raw) {
            this.raw = raw;

            if(raw.StartsWith("("))
                ParseComplex();
            else
                ParseSimple();
        }

        void ParseComplex() {
            int closeParenthesisIndex = raw.IndexOf(")");
            argument = raw.Substring(closeParenthesisIndex + 1);

            string typeString = raw.Substring(1, closeParenthesisIndex);

        }

        void ParseSimple() {
            type = null;
            argument = raw;
        }
    }
}