using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            CommandsManager.onExceptionThrown += exception=>Console.WriteLine(exception.Message);

            Configuration configuration = new Configuration("Test");
            CommandsManager manager = new CommandsManager(configuration);
            manager.LoadCommands();
            manager.Add(new CommandsBuilder(typeof(Program)).Build());
            Console.WriteLine(manager.Execute("Max (int)2 3"));
        }

        public static float Max(float a, float b) {
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