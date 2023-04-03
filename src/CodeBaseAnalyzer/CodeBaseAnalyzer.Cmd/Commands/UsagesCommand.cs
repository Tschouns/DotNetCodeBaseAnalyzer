using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.Commands.Helpers;

namespace CodeBaseAnalyzer.Cmd.Commands
{
    public class UsagesCommand : ICommand
    {
        public string GetName() => "usages";

        public string GetDescription() => "Finds and lists all usages of a source code file, i.e. projects and solutions which include the file.";

        public void DeclareParameters(IDeclareParameters declare)
        {
            declare
                .RequiredParameter("root", "The code base root directory.")
                .RequiredParameter("file", "The source code file full name, or partial name.");
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

            // First search for file, check it exists and we have a unique match.
            if (CommandTaskHelper.TryFindSingleMatchingFile(() => CodeBaseAnalyzer.Search.FindCodeFiles(codeBaseRootDirectory), fileName, out var match))
            {
                // Analyze the entire code base.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Analyzing code base root directory \"{codeBaseRootDirectory}\"...");
                var codeBase = CodeBaseAnalyzer.Graph.GenerateGraph(codeBaseRootDirectory);
                var sourceCodeFile = codeBase.SourceCodeFiles.Single(f => f.FilePath == match);

                // List usages.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Usages of \"{sourceCodeFile.FilePath}\":");

                foreach (var project in sourceCodeFile.DependentProjects)
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"> project: {project.FilePath}");
                    foreach (var solution in project.DependentSolutions)
                    {
                        ConsoleHelper.WriteLineInColor(ConsoleColor.DarkCyan, $"  > solution: {solution.FilePath}");
                    }
                }
            }
        }
    }
}
