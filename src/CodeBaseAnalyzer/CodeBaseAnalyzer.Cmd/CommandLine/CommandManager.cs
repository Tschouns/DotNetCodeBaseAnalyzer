using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Cmd.CommandLine.Internal;

namespace CodeBaseAnalyzer.Cmd.CommandLine
{
    internal class CommandManager : ICommandManager
    {
        private readonly IDictionary<string, CommandInternal> commands = new Dictionary<string, CommandInternal>();
        private readonly List<CommandInternal> commandList = new List<CommandInternal>();

        public void RegisterCommand(ICommand command)
        {
            Argument.AssertNotNull(command, nameof(command));

            var commandName = command.GetName() ?? string.Empty;
            var commandDescription = command.GetDescription() ?? string.Empty;

            if (this.commands.ContainsKey(commandName))
            {
                throw new ArgumentException($"The command {commandName} is already registered.", nameof(command));
            }

            var commandInternal = new CommandInternal(command, commandName, commandDescription);
            command.DeclareParameters(commandInternal);

            this.commands.Add(commandName, commandInternal);
            this.commandList.Add(commandInternal);
        }

        public IReadOnlyList<ICommandDescription> GetKnownCommands()
        {
            return this.commandList;
        }

        public int Execute(IReadOnlyList<string> args)
        {
            if (!args.Any())
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, "No command specified.");

                return 1;
            }

            var commandName = args.First();
            if (!this.commands.ContainsKey(commandName))
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"Unknown command \"{commandName}\".");

                return 2;
            }

            var command = this.commands[commandName];
            var remainingArguments = args.Skip(1).ToArray();

            // TODO: Special "help" param option?

            // Prepare required and optional (named) parameters.
            if (remainingArguments.Length < command.RequiredParameters.Count)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"Insufficient number of parameters (required: {command.RequiredParameters.Count}, specified: {remainingArguments.Length}).");

                return 3;
            }

            var parameterDictionary = new Dictionary<string, string>();
            var returnValue = this.PrepareParameters(command, remainingArguments, parameterDictionary);

            // Ugly...
            if (returnValue != 0)
            {
                return returnValue;
            }
  
            // Execute the command.
            try
            {
                command.Execute(parameterDictionary);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"An unexpected error has occurred while executing command \"{command.Name}\": {ex}");

                return 7;
            }

            Console.WriteLine();
            ConsoleHelper.WriteLineInColor(ConsoleColor.Green, "Done.");
            Console.WriteLine();
            Console.WriteLine();

            return 0;
        }

        private int PrepareParameters(ICommandDescription command, string[] remainingArguments, IDictionary<string, string> parameterDictionary)
        {
            Argument.AssertNotNull(command, nameof(command));
            Argument.AssertNotNull(remainingArguments, nameof(remainingArguments));
            Argument.AssertNotNull(parameterDictionary, nameof(parameterDictionary));

            // Prepare required parameters, based on order of specification.
            foreach (var parameterDescription in command.RequiredParameters)
            {
                parameterDictionary.Add(parameterDescription.Name, remainingArguments.First());
                remainingArguments = remainingArguments.Skip(1).ToArray();
            }

            // Prepare named parameters, assumed to be specified as name-value pairs.
            if (remainingArguments.Length % 2 != 0)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"Incoherent number of parameters; remaining optional (named) parameters must be an even number (name-value pairs).");

                return 4;
            }

            for (var i = 0; i < remainingArguments.Length; i += 2)
            {
                var parameterName = remainingArguments[i];
                var parameterValue = remainingArguments[i + 1];

                if (!parameterName.StartsWith('-'))
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"Parameter names must specified starting with '--', or '-' for short names (specified \"{parameterName}\").");

                    return 5;
                }

                // Determine which parameter it is.
                INamedParameterDescription parameterDescription;

                if (parameterName.StartsWith("--"))
                {
                    parameterDescription = command.NamedParameters.SingleOrDefault(p => "--" + p.Name == parameterName);
                }
                else
                {
                    parameterDescription = command.NamedParameters.SingleOrDefault(p => "-" + p.ShortName == parameterName);
                }

                if (parameterDescription == null)
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"Unknown parameter \"{parameterName}\".");

                    return 6;
                }

                parameterDictionary.Add(parameterDescription.Name, parameterValue);
            }

            return 0;
        }
    }
}
