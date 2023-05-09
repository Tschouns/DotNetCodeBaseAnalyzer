
using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Cmd.ConsoleOutput;

namespace CodeBaseAnalyzer.Cmd.CommandLine.Internal
{
    /// <summary>
    /// :)
    /// </summary>
    internal static class HelpHelper
    {
        public static void WriteCommandShortInfo(ICommandDescription command)
        {
            Argument.AssertNotNull(command, nameof(command));

            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"> {command.Name}");
            ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"  {command.Description}");
        }

        public static void WriteCommandParameters(ICommandDescription command)
        {
            Argument.AssertNotNull(command, nameof(command));

            ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"  Parameters:");

            foreach (var param in command.RequiredParameters)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"  > {param.Name} (required)");
                ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"    {param.Description}");
            }

            foreach (var param in command.NamedParameters)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"  > --{param.Name}, -{param.ShortName} (optional, named)");
                ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"    {param.Description}");
            }
        }
    }
}
