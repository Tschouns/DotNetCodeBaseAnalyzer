using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.Commands;

internal class Program
{
    private static int Main(string[] args)
    {
        // Setup commands.
        var commandManager = new CommandManager();
        commandManager.RegisterCommand(new HelpCommand(() => commandManager));
        commandManager.RegisterCommand(new AnalyzeCommand());
        commandManager.RegisterCommand(new CheckCommand());
        commandManager.RegisterCommand(new SolutionCommand());
        commandManager.RegisterCommand(new ProjectCommand());
        commandManager.RegisterCommand(new UsagesCommand());
        commandManager.RegisterCommand(new OverlapCommand());

        return commandManager.Execute(args ?? new string[0]);
    }
}