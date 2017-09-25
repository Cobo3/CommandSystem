# CommandSystem
**CommandSystem** allows you to parse strings into ready-to-use commands, making it easy to execute code, for example, from input fields present in consoles.

It is being developed as a separate module for [DevConsole](https://assetstore.unity.com/packages/tools/gui/dev-console-16833), a plugin for [Unity](https://unity3d.com/). Although I plan on using it for developing more products in the future.

## Installation
Just compile the code into a dll and reference it in your own project.
The main Assembly (CommandSystem) holds the core functionality.
The Assembly "CommandSystem-Unity" ads some special functionality to be used in Unity.

## How to
**CommandSystem** is quite simple to use. Only a few steps are needed to see it fully working:
1. Create a Commands Manager
2. Create and add Commands
3. Execute a Command
4. \(Optional) Create custom Parsers

### Create a Commands Manager
The class CommandsManager is the main interface through the system, and you'll need one if you plan on using it.
```C#
using SickDev.CommandSystem;

static void Main(string[] args){
    CommandsManager manager = new CommandsManager();
}
```

### Create and add Commands
Commands can be created in three differet ways.
- Manually
- Using the CommandAttribute
- Using the CommandsBuilder

#### Manually
The good thing about using this method is that you can add and remove Commands at runtime; which can be good for, say, a text-based videogame.
Another good thing about this method is that you can use instance methods.
The bad thing, however, is that you have to do it manually.

Using this method, you first need to create the command and then add it to the CommandsManager. A command can be created using one of the different CommandType classes, depending on your needs. CommandTypes are separated between ActionCommands and FuncCommands, just like .NET delegates.

```C#
using SickDev.CommandSystem;

static void Main(string[] args){
    CommandsManager manager = new CommandsManager();
    
    Command actionCommand = new ActionCommand<int>(ExampleActionCommand);
    Command funcCommand = new FuncCommand<int, string, bool>(ExampleFuncCommand);
    
    manager.Add(actionCommand);
    manager.Add(funcCommand);
}

static void ExampleActionMethod(int number){
    Console.WriteLine ("The input number is "+(number%2 == 0?"even":"odd"));
}

static bool ExampleFuncCommand(int number, string stringNumber){
    int parsedNumber;
    return int.TryParse(stringNumber, out parsedNumber) && parsedNumber == number;
}
```

                   Rename Commands    Static Members    Instance Members    Methods    Properties    Variables    Runtime
Manually                  O                 O                   O              O           X             X           O
CommandAttribute          O                 O                   X              O           X             X           X
CommandsBuilder           X                 O                   X              O           O             O           O
(Add use delegates)
