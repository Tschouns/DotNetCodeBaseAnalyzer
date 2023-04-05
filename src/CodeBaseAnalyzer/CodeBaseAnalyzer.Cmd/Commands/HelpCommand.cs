using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Graph.Model;

namespace CodeBaseAnalyzer.Cmd.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly Func<ICommandManager> getCommandManager;

        public HelpCommand(Func<ICommandManager> getCommandManager)
        {
            Argument.AssertNotNull(getCommandManager, nameof(getCommandManager));

            this.getCommandManager = getCommandManager;
        }

        public string GetName() => "help";

        public string GetDescription() => "Displays helpful information on available commands.";

        public void DeclareParameters(IDeclareParameters declare)
        {
            declare.NamedParameter("command", "c", "A command for which to get detailed information.");
        }

        public void Execute(IDictionary<string, string> parametersByName)
        {
            var commandManager = this.getCommandManager();
            var commands = commandManager.GetKnownCommands();

            // Details on specific command?
            if (parametersByName.TryGetValue("command", out var commandName))
            {
                var specificCommand = commands.SingleOrDefault(c => c.Name == commandName);
                if (specificCommand == null)
                {
                    throw new CommandException($"The specified command \"{commandName}\" is unknown.");
                }

                this.WriteShortInfo(specificCommand);
                this.WriteParameters(specificCommand);

                return;
            }

            // Otherwise, quick overview of all commands.
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, "Available commands:");

            foreach ( var command in commands )
            {
                this.WriteShortInfo(command);
            }
        }

        private void WriteShortInfo(ICommandDescription command)
        {
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"> {command.Name}");
            ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan,  $"  {command.Description}");
        }

        private void WriteParameters(ICommandDescription command)
        {
            ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"  Parameters:");

            foreach (var param in command.RequiredParameters)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"  > {param.Name} (required)");
                ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan,  $"    {param.Description}");
            }

            foreach (var param in command.NamedParameters)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"  > --{param.Name}, -{param.ShortName} (optional, named)");
                ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan,  $"    {param.Description}");
            }
        }
    }
}
