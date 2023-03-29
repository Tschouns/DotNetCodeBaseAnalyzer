using CodeBaseAnalyzer.Cmd;
using CodeBaseAnalyzer.Issues;

internal class Program
{
    private static int Main(string[] args)
    {
        var defaultColor = Console.ForegroundColor;

        if (args.Length != 1)
        {
            ConsoleHelper.WriteLineInColor(ConsoleColor.Red, "Exactly 1 argument is expected: \"code base root directory\"");

            return 1;
        }
        
        var codeBaseRootDirectory = args.Single();
        if (!Directory.Exists(codeBaseRootDirectory))
        {
            ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"The code base root directory \"{codeBaseRootDirectory}\" does not exist.");

            return 2;
        }

        // Generate the graph
        ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Analyzing code base root directory \"{codeBaseRootDirectory}\", generating graph...");

        try
        {
            var codeBase = CodeBaseAnalyzer.CodeBaseAnalyzer.Graph.GenerateGraph(codeBaseRootDirectory);

            ConsoleHelper.WriteLineInColor(ConsoleColor.White, "Code base graph generated.");

            Console.WriteLine();
            Console.WriteLine();

            // Order and print issues.
            var issues = codeBase.Issues
                .OrderBy(i => i.Type)
                .ThenBy(i => i.Message)
                .ToList();

            var lineNumber = 1;

            foreach (var issue in issues)
            {
                var color = DetermineColorByIssueType(issue.Type, defaultColor);
                ConsoleHelper.WriteLineInColor(color, $"{string.Format("{0,5:#####}", lineNumber)}\t{issue.Type}:\t{issue.Message}");
                lineNumber++;
            }

            Console.WriteLine();
            Console.WriteLine();

            // Print a summary.
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, "=== Summary ===");
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"{codeBase.Solutions.Count()} solution(s) found.");
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"{codeBase.Projects.Count()} project(s) found.");
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"{codeBase.SourceCodeFiles.Count()} source code file(s) found.");
            Console.WriteLine();
            ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"{issues.Count(i => i.Type == IssueType.Error)} error(s) found.");
            ConsoleHelper.WriteLineInColor(ConsoleColor.Yellow, $"{issues.Count(i => i.Type == IssueType.Warning)} warning(s) found.");

            return 0;
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLineInColor(ConsoleColor.Red, $"An unexpected error has occurred: {ex}");

            return 3;
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