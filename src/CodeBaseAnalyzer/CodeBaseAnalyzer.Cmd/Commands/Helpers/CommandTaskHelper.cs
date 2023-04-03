using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Cmd.Commands.Helpers
{
    internal static class CommandTaskHelper
    {
        public static bool TryFindSingleMatchingFile(Func<IEnumerable<string>> searchForFiles, string fileNamePart, out string? match)
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
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"No file matching \"{fileNamePart}\" was found.");
                match = null;

                return false;
            }

            if (matchingCodeFiles.Count() > 1)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"Multiple files matching \"{fileNamePart}\" were found:");

                foreach (var file in matchingCodeFiles)
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.Magenta, file);
                }

                match = null;

                return false;
            }

            match = matchingCodeFiles.Single();
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Found matching file \"{match}\".");

            return true;
        }

        public static void ListIssuesInColor(IEnumerable<Issue> issues)
        {
            var defaultColor = Console.ForegroundColor;
            var lineNumber = 1;

            foreach (var issue in issues)
            {
                var color = DetermineColorByIssueType(issue.Type, defaultColor);
                ConsoleHelper.WriteLineInColor(color, $"{string.Format("{0,5:#####}", lineNumber)}\t{issue.Type}:\t{issue.Message}");
                lineNumber++;
            }
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
