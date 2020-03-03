using System.Linq;
using System.Reflection;

namespace SickDev.CommandSystem
{
	public class Signature
	{
		Command command;
		ParameterInfo[] parameters;
		int nonOptionalParameters;
		string _raw;

		public string raw => _raw ?? (_raw = SignatureBuilder.Build(command.method, command.name));

		internal Signature(Command command)
		{
			this.command = command;
			parameters = command.method.GetParameters();
			nonOptionalParameters = parameters.Count(x => !x.IsOptional);
		}

		internal bool Matches(ParsedArgument[] args) => args.Length >= nonOptionalParameters && args.Length <= parameters.Length;

		internal object[] Convert(ParsedArgument[] args, ArgumentsParser parser)
		{
			object[] oArgs = new object[parameters.Length];
			for (int i = 0; i < oArgs.Length; i++)
			{
				//This should be always true...
				if (args.Length > i)
					oArgs[i] = parser.Parse(args[i], parameters[i].ParameterType);
				//Except for when there are optional parameters for which no argument has been provided
				else
					oArgs[i] = parameters[i].DefaultValue;
			}
			return oArgs;
		}
	}
}