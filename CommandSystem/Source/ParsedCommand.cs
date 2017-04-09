using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class ParsedCommand {
        public string raw { get; private set; }
        public string command { get; private set; }
        public string[] args { get; private set; }

        static readonly char[] groupifiers = { '\'', '\"' };
        const char separator = ' ';

        public ParsedCommand(string raw) {
            this.raw = raw;
            GetCommand();
            GetArgs();
        }

        void GetCommand() {
            string[] parts = raw.Split(' ');
            command = parts[0];
        }
        
        void GetArgs() {
            string sArgs = raw.Substring(command.Length).Trim();
            List<string> lArgs = new List<string>();

            char? groupifier = null;
            string arg = string.Empty;
            for (int i = 0; i < sArgs.Length; i++) {
                if (sArgs[i] == separator && groupifier == null) {
                    lArgs.Add(arg);
                    arg = string.Empty;
                    continue;
                }

                bool isGroupifier = false;
                for (int j = 0; j < groupifiers.Length; j++) {
                    if (sArgs[i] == groupifiers[j]) {
                        isGroupifier = true;
                        if (groupifier == null)
                            groupifier = groupifiers[j];
                        else if (groupifier == sArgs[i]) {
                            lArgs.Add(arg);
                            arg = string.Empty;
                            groupifier = null;
                        }
                    }
                }

                if (!isGroupifier)
                    arg += sArgs[i];
            }

            if (arg != string.Empty)
                lArgs.Add(arg);
            args = lArgs.ToArray();
        }
    }
}