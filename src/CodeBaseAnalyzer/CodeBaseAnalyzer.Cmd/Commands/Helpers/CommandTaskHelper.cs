using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.ConsoleOutput;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Cmd.Commands.Helpers
{
    internal static class CommandTaskHelper
    {
        /// <summary>
        /// Finds a single file matching the specified file name (or part) within a search result set, determined by
        /// the specified search function.
        /// </summary>
        /// <param name="searchForFiles">
        /// The search function which returns the base set of files to search
        /// </param>
        /// <param name="fileNamePart">
        /// The file name (or part) to match
        /// </param>
        /// <returns>
        /// The single matching file
        /// </returns>
        /// <exception cref="CommandException">
        /// Thrown if the match could not be uniquely determined
        /// </exception>
        public static string FindSingleMatchingFile(Func<IEnumerable<string>> searchForFiles, string fileNamePart)
        {
            Argument.AssertNotNull(searchForFiles, nameof(searchForFiles));
            Argument.AssertNotNull(fileNamePart, nameof(fileNamePart));

            // First search for file, check it exists and we have a unique match.
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Searching for \"{fileNamePart}\"...");
            var allFiles = searchForFiles();

            var matchingCodeFiles = allFiles
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

        /// <summary>
        /// Displays the specified list of issues -- in color.
        /// </summary>
        /// <param name="issues">
        /// The issues to display
        /// </param>
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

        /// <summary>
        /// Displays a summary of the specified issues.
        /// </summary>
        /// <param name="issues">
        /// The issues for which to display the summary
        /// </param>
        public static void PrintIssuesSummary(IEnumerable<Issue> issues)
        {
            Argument.AssertNotNull(issues, nameof(issues));

            var errorCount = issues.Count(i => i.Type == IssueType.Error);
            var warningCount = issues.Count(i => i.Type == IssueType.Warning);

            ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"{errorCount} error(s) found.");
            ConsoleHelper.WriteLineInColor(ConsoleColor.Yellow, $"{warningCount} warning(s) found.");
        }

        /// <summary>
        /// Determines the display color for the specified issue type.
        /// </summary>
        /// <param name="issueType">
        /// The issue type
        /// </param>
        /// <param name="defaultColor">
        /// The default color, used as fall-back
        /// </param>
        /// <returns>
        /// The appropriate display color
        /// </returns>
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
