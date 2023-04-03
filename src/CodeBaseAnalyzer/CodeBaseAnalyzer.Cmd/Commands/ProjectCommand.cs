using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.Commands.Helpers;

namespace CodeBaseAnalyzer.Cmd.Commands
{
    public class ProjectCommand : ICommand
    {
        public string GetName() => "project";

        public string GetDescription() => "Displays information on a specific project.";

        public void DeclareParameters(IDeclareParameters declare)
        {
            declare
                .RequiredParameter("root", "The code base root directory.")
                .RequiredParameter("file", "The project file full name, or partial name.");
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

            if (CommandTaskHelper.TryFindSingleMatchingFile(() => CodeBaseAnalyzer.Search.FindProjectFiles(codeBaseRootDirectory), fileName, out var match))
            {
                // Analyze the entire code base.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Analyzing code base root directory \"{codeBaseRootDirectory}\"...");
                var codeBase = CodeBaseAnalyzer.Graph.GenerateGraph(codeBaseRootDirectory);
                var project = codeBase.Projects.Single(f => f.FilePath == match);

                Console.WriteLine();
                Console.WriteLine();

                // Referenced projects.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Projects referenced by \"{project.FilePath}\":");
                foreach (var referencedProject in project.ReferencedProjects)
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"> {referencedProject.FilePath}");
                }

                Console.WriteLine();
                Console.WriteLine();

                // Dependent projects.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Dependent projects, referencing \"{project.FilePath}\":");
                foreach (var dependentProject in project.DependentProjects)
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"> {dependentProject.FilePath}");
                }

                Console.WriteLine();
                Console.WriteLine();

                // Issues.
                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Known issues with \"{project.FilePath}\":");
                CommandTaskHelper.ListIssuesInColor(project.Issues);
            }
        }
    }
}
