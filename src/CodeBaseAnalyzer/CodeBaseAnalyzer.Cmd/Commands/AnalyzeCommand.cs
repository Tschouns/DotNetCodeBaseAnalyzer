using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.Commands.Helpers;
using CodeBaseAnalyzer.Cmd.ConsoleOutput;

namespace CodeBaseAnalyzer.Cmd.Commands
{
    public class AnalyzeCommand : ICommand
    {
        public string GetName() => "analyze";

        public string GetDescription() => "Analyzes a code base, and displays all detected issues.";

        public void DeclareParameters(IDeclareParameters declare)
        {
            declare.RequiredParameter("root", "The code base root directory.");
        }

        public void Execute(IDictionary<string, string> parametersByName)
        {
            var codeBaseRootDirectory = parametersByName["root"];

            if (!Directory.Exists(codeBaseRootDirectory))
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"The code base root directory \"{codeBaseRootDirectory}\" does not exist.");

                return;
            }

            // Generate the graph
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Analyzing code base root directory \"{codeBaseRootDirectory}\", generating graph...");

            var codeBase = CodeBaseAnalyzer.Graph.GenerateGraph(codeBaseRootDirectory);

            ConsoleHelper.WriteLineInColor(ConsoleColor.White, "Code base graph generated.");

            Console.WriteLine();
            Console.WriteLine();

            // Order and print issues.
            var issues = codeBase.Issues
                .OrderBy(i => i.Type)
                .ThenBy(i => i.Message)
                .ToList();

            CommandTaskHelper.ListIssuesInColor(issues);

            Console.WriteLine();
            Console.WriteLine();

            // Print a summary.
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"{codeBase.Solutions.Count()} solution(s) found.");
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"{codeBase.Projects.Count()} project(s) found.");
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"{codeBase.SourceCodeFiles.Count()} source code file(s) found.");
            Console.WriteLine();

            CommandTaskHelper.PrintIssuesSummary(issues);
        }
    }
}
