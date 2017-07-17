using System;
using SickDev.CommandSystem;

namespace Test {
    class Program {
        static void Main(string[] args) {
            CommandsManager.onMessage += Console.WriteLine;
            Config.AddAssemblyWithCommands("Test");

            CommandsManager manager = new CommandsManager();
            manager.Load();
            manager.Add(new CommandsBuilder(typeof(Program)).Build());
            Console.WriteLine(manager.Execute("GetEnumValues Cosa"));
        }

        [Command]
        public static int Test(params int[] n) {
            return n.Length;
        }

        enum Cosa { tree, doo, quaaaa}
    }

    enum Cosa { tree2, doo2, quaaaa2}
}