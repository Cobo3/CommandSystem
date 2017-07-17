using System;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    static class BuiltInCommands {
        [Command]
        static string GetEnumValues(string enumName) {
            string text = string.Empty;
            Type[] types = ReflectionFinder.LoadEnums();
            List<Type> matches = new List<Type>();
            for(int i = 0; i < types.Length; i++) {
                if(!types[i].IsEnum)
                    continue;

                if(types[i].Name.Equals(enumName, StringComparison.OrdinalIgnoreCase))
                    matches.Add(types[i]);
            }

            for(int i = 0; i < matches.Count; i++) {
                string[] values = Enum.GetNames(matches[i]);
                text += matches[i].FullName.Replace('+','.')+":\n";
                for(int j = 0; j < values.Length; j++)
                    text += "\t" + values[j] + "\n";
            }
            return text;
        }
    }
}
