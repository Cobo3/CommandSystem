﻿using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;

            CommandsManager manager = new CommandsManager(new Configuration("Test"));
            manager.Load();
            Command command = new FuncCommand<int, string, bool>(new Program().ExampleFuncCommand);
            manager.Add(command);
            Console.WriteLine(manager.Execute("ExampleFuncCommand 2 3"));
            manager.Remove(command);
            Console.WriteLine(manager.Execute("ExampleFuncCommand 2 3"));
        }

        bool ExampleFuncCommand(int number, string stringNumber) {
            int parsedNumber;
            return int.TryParse(stringNumber, out parsedNumber) && parsedNumber == number;
        }
    }
}