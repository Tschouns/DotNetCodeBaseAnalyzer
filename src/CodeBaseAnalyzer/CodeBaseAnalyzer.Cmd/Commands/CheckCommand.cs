
using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.Commands.Helpers;
using CodeBaseAnalyzer.Cmd.ConsoleOutput;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Cmd.Commands
{
    public class CheckCommand : ICommand
    {
        public string GetName() => "check";

        public string GetDescription() => "Analyses a code base or specific solution, and fails (returns a non-zero value) if there are any errors. This may be useful automated builds / CI pipelines.";

        public void DeclareParameters(IDeclareParameters declare)
        {
            declare
                .RequiredParameter("root", "The code base root directory.")
                .NamedParameter("solution", "s", "The solution file full name, or partial name.");
        }

        public void Execute(IDictionary<string, string> parametersByName)
        {
            var codeBaseRootDirectory = parametersByName["root"];

            if (!Directory.Exists(codeBaseRootDirectory))
            {
                throw new CommandException($"The code base root directory \"{codeBaseRootDirectory}\" does not exist.");
            }

            if (parametersByName.TryGetValue("solution", out var solutionFileName))
            {
                var match = CommandTaskHelper.FindSingleMatchingFile(() => CodeBaseAnalyzer.Search.FindSolutionFiles(codeBaseRootDirectory), solutionFileName);

                // Analyze the entire code base, select matching solution.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Analyzing solution \"{match}\"...");

                var codeBase = CodeBaseAnalyzer.Graph.GenerateGraph(codeBaseRootDirectory);
                var solution = codeBase.Solutions.Single(f => f.FilePath == match);

                this.CheckIssues(solution.Issues);
            }
            else
            {
                // Analyze the entire code base.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Analyzing code base root directory \"{codeBaseRootDirectory}\"...");

                var codeBase = CodeBaseAnalyzer.Graph.GenerateGraph(codeBaseRootDirectory);

                this.CheckIssues(codeBase.Issues);
            }
        }

        private void CheckIssues(IEnumerable<Issue> issues)
        {
            Console.WriteLine();
            Console.WriteLine();

            // Issues.
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, "Known issues:");
            CommandTaskHelper.ListIssuesInColor(issues);

            Console.WriteLine();
            Console.WriteLine();

            CommandTaskHelper.PrintIssuesSummary(issues);

            // Fail the command if there are any errors.
            if (issues.Any(i => i.Type == IssueType.Error))
            {
                throw new CommandException($"The solution analysis yielded {issues.Count(i => i.Type == IssueType.Error)} error(s).");
            }
        }
    }
}
