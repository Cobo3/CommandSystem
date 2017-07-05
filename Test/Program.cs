using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            Config.AddAssemblyWithCommands("Test");

            CommandsManager manager = new CommandsManager();
            manager.Load();
            Console.WriteLine(manager.GetCommandExecuter("LogRepeated \"hola farola\"10").Execute());
        }

        [Command]
        static void LogRepeated(string text, int count) {
            for(int i = 0; i < count; i++)
                Console.WriteLine(text);
        }
    }
}