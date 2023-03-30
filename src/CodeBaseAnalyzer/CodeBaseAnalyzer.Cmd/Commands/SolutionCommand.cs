using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.Commands.Helpers;

namespace CodeBaseAnalyzer.Cmd.Commands
{
    public class SolutionCommand : ICommand
    {
        public string GetName() => "solution";

        public string GetDescription() => "Displays information on a specific solution.";

        public void DeclareParameters(IDeclareParameters declare)
        {
            declare
                .RequiredParameter("root", "The code base root directory.")
                .RequiredParameter("file", "The solution file full name, or partial name.");
        }

        public void Execute(IDictionary<string, string> parametersByName)
        {
            var codeBaseRootDirectory = parametersByName["root"];
            var fileName = parametersByName["file"];

            if (!Directory.Exists(codeBaseRootDirectory))
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"The code base root directory \"{codeBaseRootDirectory}\" does not exist.");
                return;
            }

            if (CommandTaskHelper.TryFindSingleMatchingFile(() => CodeBaseAnalyzer.Search.FindSolutionFiles(codeBaseRootDirectory), fileName, out var match))
            {
                // Analyze the entire code base.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Analyzing code base root directory \"{codeBaseRootDirectory}\"...");
                var codeBase = CodeBaseAnalyzer.Graph.GenerateGraph(codeBaseRootDirectory);
                var solution = codeBase.Solutions.Single(f => f.FilePath == match);

                Console.WriteLine();
                Console.WriteLine();

                // Projects.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Projects included in \"{solution.FilePath}\":");
                foreach (var project in solution.IncludedProjects)
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"> {project.FilePath}");
                }

                Console.WriteLine();
                Console.WriteLine();

                // Issues.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Known issues with \"{solution.FilePath}\":");
                CommandTaskHelper.ListIssuesInColor(solution.Issues);
            }
        }
    }
}
