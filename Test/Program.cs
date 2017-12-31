using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            CommandsManager.onExceptionThrown += exception=>Console.WriteLine(exception.Message);

            CommandsManager manager = new CommandsManager(new Configuration("Test"));
            manager.Load();
            Command command = new FuncCommand<int, string, bool>(new Program().ExampleFuncCommand);
            manager.Add(command);
            Console.WriteLine(manager.Execute("ExampleFuncCommand 2 3"));
            manager.Remove(command);
            manager.Add(new CommandsBuilder(typeof(Program)).Build());
            Console.WriteLine(manager.Execute("Max (int)2 (float)3"));
        }

        bool ExampleFuncCommand(int number, string stringNumber) {
            int parsedNumber;
            return int.TryParse(stringNumber, out parsedNumber) && parsedNumber == number;
        }

        public static float Max(float a, float b) {
            if(a > b)
                return a;
            else
                return b;
        }

        public static int Max(int a, int b) {
            if(a > b)
                return a;
            else
                return b;
        }

        public static int Max(bool a, bool b) {
            return 2;
        }
    }
}