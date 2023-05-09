using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.CommandLine.Internal;
using CodeBaseAnalyzer.Cmd.ConsoleOutput;

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

                HelpHelper.WriteCommandShortInfo(specificCommand);
                HelpHelper.WriteCommandParameters(specificCommand);

                return;
            }

            // Otherwise, quick overview of all commands.
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, "Available commands:");

            foreach (var command in commands)
            {
                HelpHelper.WriteCommandShortInfo(command);
            }
        }
    }
}
