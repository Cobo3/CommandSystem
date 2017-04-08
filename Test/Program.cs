using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            CommandsManager manager = new CommandsManager();
            manager.AddAssemblyWithCommands("Test.exe");
            manager.Load();
            Console.WriteLine(manager.GetCommandExecuter("test").Execute());
        }

        [Command(alias ="test")]
        static string DomainInfo() {
            return AppDomain.CurrentDomain.ToString();
        }
    }
}
