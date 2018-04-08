using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static CommandsManager manager;
        static Command command;
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            CommandsManager.onExceptionThrown += exception=>Console.WriteLine(exception.Message);

            Configuration configuration = new Configuration("Test", "CommandSystem-Unity");
            manager = new CommandsManager(configuration);
            manager.LoadCommands();
            Test test = new Test(manager);
            test.Build();
            //manager.Add(new CommandsBuilder(typeof(Program)).Build());
            manager.Add(command = new FuncCommand<float, float, float>(Max));
            Console.WriteLine(manager.Execute("Max 2 3"));
            Console.WriteLine(manager.Execute("Max 2 3"));
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

        class Test : SickDev.CommandSystem.Unity.BuiltInCommandsBuilder {
            public Test(CommandsManager manager) : base(manager) { }

            public override void Build() {
                SystemInfo();
            }
        }
    }
}