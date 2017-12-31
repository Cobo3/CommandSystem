using System;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class ParsedCommand {
        public string raw { get; private set; }
        public string command { get; private set; }
        public ParsedArgument[] args { get; private set; }

        static readonly char[] groupifiers = { '\'', '\"' };
        const char separator = ' ';

        public ParsedCommand(string raw) {
            this.raw = raw;
            GetCommand();
            GetArgs();
        }

        void GetCommand() {
            string[] parts = raw.Split(separator);
            command = parts[0];
        }
        
        void GetArgs() {
            string stringArgs = raw.Substring(command.Length).Trim();
            List<string> listArgs = new List<string>();

            char? groupifier = null;
            string arg = string.Empty;
            for (int i = 0; i < stringArgs.Length; i++) {
                if (stringArgs[i] == separator && groupifier == null) {
                    if(!string.IsNullOrEmpty(arg)) {
                        listArgs.Add(arg);
                        arg = string.Empty;
                    }
                    continue;
                }

                bool isGroupifier = false;
                for (int j = 0; j < groupifiers.Length; j++) {
                    if (stringArgs[i] == groupifiers[j]) {
                        isGroupifier = true;
                        if (groupifier == null)
                            groupifier = groupifiers[j];
                        else if (groupifier == stringArgs[i]) {
                            listArgs.Add(arg);
                            arg = string.Empty;
                            groupifier = null;
                        }
                    }
                }

                if (!isGroupifier)
                    arg += stringArgs[i];
            }

            if (arg != string.Empty)
                listArgs.Add(arg);
            args = listArgs.ConvertAll(x=>new ParsedArgument(x)).ToArray();
        }
    }
}