using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static CommandsManager manager;
        static Command command;
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            CommandsManager.onExceptionThrown += exception=>Console.WriteLine(exception.Message);

            Configuration configuration = new Configuration(true, "Test", "CommandSystem-Unity");
            manager = new CommandsManager(configuration);
            manager.LoadCommands();
            Func<float, float, float> action = (a, b)=> {
                if(a > b)
                    return a;
                else
                    return b;
            };
            manager.Add(command = new FuncCommand<float, float, float>(action));
            manager.Add(command = new FuncCommand<float[], float>(Max));
            manager.Add(command = new FuncCommand<int[], float>(Max));
            Console.WriteLine(manager.Execute("Max \"(int[])2 3\""));
        }

        public static float Max(float[] numbers) {
            return Max(numbers[0], numbers[1]);
        }

        public static float Max(int[] numbers) {
            return Max(numbers[0], numbers[1]);
        }

        public static float Max(float a, float b) {
            manager.Remove(command);
            if(a > b)
                return a;
            else
                return b;
        }

        public static float Max(int a, float b) {
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