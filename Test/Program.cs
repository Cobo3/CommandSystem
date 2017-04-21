using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            Config.AddAssemblyWithCommands("Test");

            CommandsManager manager = new CommandsManager();
            manager.Load();
            Console.WriteLine(manager.GetCommandExecuter("test").Execute());
        }

        [Command(alias = "test")]
        static string TestMethod() {
            return "this text should be shown on the console when executing the command";
        }
    }
}
