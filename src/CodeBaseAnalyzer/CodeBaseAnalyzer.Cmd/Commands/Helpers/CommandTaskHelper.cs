using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Cmd.Commands.Helpers
{
    internal static class CommandTaskHelper
    {
        public static string FindSingleMatchingFile(Func<IEnumerable<string>> searchForFiles, string fileNamePart)
        {
            Argument.AssertNotNull(searchForFiles, nameof(searchForFiles));
            Argument.AssertNotNull(fileNamePart, nameof(fileNamePart));

            // First search for file, check it exists and we have a unique match.
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Searching for \"{fileNamePart}\"...");
            var allCodeFiles = searchForFiles();

            var matchingCodeFiles = allCodeFiles
                .Where(f => f.Contains(fileNamePart, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchingCodeFiles.Any())
            {
                throw new CommandException($"No file matching \"{fileNamePart}\" was found.");                
            }

            if (matchingCodeFiles.Count() > 1)
            {
                var message = $"Multiple files matching \"{fileNamePart}\" were found:\n{string.Join('\n', matchingCodeFiles)}";

                throw new CommandException(message);
            }

            var match = matchingCodeFiles.Single();
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Found matching file \"{match}\".");

            return match;
        }

        public static void ListIssuesInColor(IEnumerable<Issue> issues)
        {
            Argument.AssertNotNull(issues, nameof(issues));

            var defaultColor = Console.ForegroundColor;
            var lineNumber = 1;

            foreach (var issue in issues)
            {
                var color = DetermineColorByIssueType(issue.Type, defaultColor);
                ConsoleHelper.WriteLineInColor(color, $"{string.Format("{0,5:#####}", lineNumber)}\t{issue.Type}:\t{issue.Message}");
                lineNumber++;
            }
        }

        public static void PrintIssuesSummary(IEnumerable<Issue> issues)
        {
            Argument.AssertNotNull(issues, nameof(issues));

            var errorCount = issues.Count(i => i.Type == IssueType.Error);
            var warningCount = issues.Count(i => i.Type == IssueType.Warning);

            ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"{errorCount} error(s) found.");
            ConsoleHelper.WriteLineInColor(ConsoleColor.Yellow, $"{warningCount} warning(s) found.");
        }

        private static ConsoleColor DetermineColorByIssueType(IssueType issueType, ConsoleColor defaultColor)
        {
            switch (issueType)
            {
                case IssueType.Error: return ConsoleColor.Red;
                case IssueType.Warning: return ConsoleColor.Yellow;
                case IssueType.Info: return ConsoleColor.White;
                default: return defaultColor;
            }
        }
    }
}
