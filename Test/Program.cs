using System;
using System.Threading;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager manager = new CommandsManager();
            CommandsManager.onMessage += Console.WriteLine;
            manager.Load();
            Console.WriteLine(manager.GetCommandExecuter("DomainInfo").Execute());
            while (true)
                Thread.Sleep(5000);
        }

        [Command]
        static string DomainInfo() {
            return AppDomain.CurrentDomain.ToString();
        }
    }
}
