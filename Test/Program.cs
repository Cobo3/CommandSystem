using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            CommandsManager manager = new CommandsManager();
            manager.AddAssemblyWithCommands("Test.exe");
            manager.Add(new FuncCommand<string>(TestMethod));
            Console.WriteLine(manager.GetCommandExecuter("TestMethod").Execute());
        }

        [Command(alias = "test")]
        static string TestMethod() {
            return "this text should be shown on the console when executing the command";
        }
    }
}
