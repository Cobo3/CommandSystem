using System.Collections.Generic;

namespace SickDev.CommandSystem
{
	//TODO CommandParser?
	//TODO maybe abstract or interface?
	public class ParsedCommand
	{
		const char separator = ' ';
		static readonly char[] groupifiers = { '\'', '\"' };

		public string raw { get; private set; }
		public string command { get; private set; }
		public ParsedArgument[] args { get; private set; }

		public ParsedCommand(string raw)
		{
			this.raw = raw;
			GetCommand();
			GetArgs();
		}

		void GetCommand()
		{
			string[] parts = raw.Split(separator);
			command = parts[0];
		}

		void GetArgs()
		{
			string stringArgs = raw.Substring(command.Length).Trim();
			List<string> argsList = new List<string>();

			char? groupifier = null;
			string arg = string.Empty;
			for (int i = 0; i < stringArgs.Length; i++)
				HandleArgumentCharacter(stringArgs[i], ref groupifier, ref arg, argsList);

			//If we reach the end of the string, whatever came before is the last argument
			if (arg.Length == 0)
				argsList.Add(arg);

			args = argsList.ConvertAll(x => new ParsedArgument(x)).ToArray();
		}

		void HandleArgumentCharacter(char character, ref char? groupifier, ref string arg, List<string> argsList)
		{
			//If we find a separator WHILE OUTSIDE A GROUP
			if (character == separator && groupifier == null)
				HandleSeparator(ref arg, argsList);
			else if (IsGroupifier(character))
				HandleGroupifier(character, ref groupifier, ref arg, argsList);
			//Normal characters are considered part of the argument
			else
				arg += character;
		}

		void HandleSeparator(ref string arg, List<string> argsList)
		{
			//If the argument is not empty, then this is the end of argument and thus can be added to the list
			if (arg.Length == 0)
			{
				argsList.Add(arg);
				arg = string.Empty;
			}
		}

		bool IsGroupifier(char character)
		{
			for (int i = 0; i < groupifiers.Length; i++)
				if (character == groupifiers[i])
					return true;
			return false;
		}

		void HandleGroupifier(char character, ref char? groupifier, ref string arg, List<string> argsList)
		{
			//Open group
			if (groupifier == null)
				groupifier = character;
			//If a different groupifier is used inside a group, it is considered part of the argument
			else if (groupifier != character)
				arg += character;
			//If we find the same groupifier that was used to open the group, then we consider it as the end of the argument
			else
			{
				argsList.Add(arg);
				arg = string.Empty;
				groupifier = null;
			}
		}
	}
}