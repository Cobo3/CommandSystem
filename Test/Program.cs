using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            Config.RegisterAssembly("Test");

            CommandsManager manager = new CommandsManager();
            manager.Load();
            manager.Add(new FuncCommand<int, string, bool>(new Program().ExampleFuncCommand));
            Console.WriteLine(manager.Execute("ExampleFuncCommand 2 3"));
        }

        bool ExampleFuncCommand(int number, string stringNumber) {
            int parsedNumber;
            return int.TryParse(stringNumber, out parsedNumber) && parsedNumber == number;
        }
    }
}