using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            Config.AddAssemblyWithCommands("Test");

            CommandsManager manager = new CommandsManager();
            manager.Load();
            manager.Add(new CommandsBuilder(typeof(Program)).Build());
        }
    }
}