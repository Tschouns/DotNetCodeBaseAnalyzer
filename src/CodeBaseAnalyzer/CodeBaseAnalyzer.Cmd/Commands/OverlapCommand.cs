using CodeBaseAnalyzer.Cmd.CommandLine;
using CodeBaseAnalyzer.Cmd.Commands.Helpers;
using CodeBaseAnalyzer.Cmd.ConsoleOutput;
using CodeBaseAnalyzer.Graph.Model;

namespace CodeBaseAnalyzer.Cmd.Commands
{
    public class OverlapCommand : ICommand
    {
        public string GetName() => "overlap";

        public string GetDescription() => "Determines the overlap in project references between one solution and the other solutions in the code base.";

        public void DeclareParameters(IDeclareParameters declare)
        {
            declare
                .RequiredParameter("root", "The code base root directory.")
                .RequiredParameter("solution", "The solution (file full name, or partial name).")
                .NamedParameter("exclude", "e", "Solution file names/paths to exclude from the analysis.");
        }

        public void Execute(IDictionary<string, string> parametersByName)
        {
            var codeBaseRootDirectory = parametersByName["root"];
            var solutionFileName = parametersByName["solution"];

            if (!Directory.Exists(codeBaseRootDirectory))
            {
                throw new CommandException($"The code base root directory \"{codeBaseRootDirectory}\" does not exist.");
            }

            var solutionMatch = CommandTaskHelper.FindSingleMatchingFile(() => CodeBaseAnalyzer.Search.FindSolutionFiles(codeBaseRootDirectory), solutionFileName);

            // Analyze the entire code base.
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Analyzing code base root directory \"{codeBaseRootDirectory}\"...");
            var codeBase = CodeBaseAnalyzer.Graph.GenerateGraph(codeBaseRootDirectory);
            var solution = codeBase.Solutions.Single(s => s.FilePath == solutionMatch);

            Console.WriteLine();
            Console.WriteLine();

            // Excluded solutions.
            var excludedSolutions = new List<ISolution>();
            if (parametersByName.TryGetValue("exclude", out var excludeString))
            {
                var excludedPartialPaths = excludeString.Split(';').Select(s => s.Trim()).ToList();
                excludedSolutions = codeBase.Solutions
                    .Where(s => excludedPartialPaths.Any(p => s.FilePath.Contains(p, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Excluding solutions... :");

                foreach (var excludedSolution in excludedSolutions)
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"> {excludedSolution.FilePath}");
                }

                Console.WriteLine();
                Console.WriteLine();
            }

            // Exclusive projects, only referenced by this solution.
            var exclusiveProjects = solution.IncludedProjects
                .Where(p => p.DependentSolutions.Count(s => !excludedSolutions.Contains(s)) == 1)
                .ToList();

            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Exclusive projects -- only included in \"{solution.FilePath}\":");
            foreach (var project in exclusiveProjects)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"> {project.FilePath}");
            }

            Console.WriteLine();
            Console.WriteLine();

            // Project with overlap.
            var overlapProjects = solution.IncludedProjects
                .Where(p => p.DependentSolutions.Count(s => !excludedSolutions.Contains(s)) > 1)
                .ToList();

            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Projects also included in other solutions:");
            foreach (var project in overlapProjects)
            {
                ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"> {project.FilePath} -- included in {project.DependentSolutions.Count - 1} other solution(s):");
                foreach (var otherSolution in project.DependentSolutions.Where(s => s != solution))
                {
                    ConsoleHelper.WriteLineInColor(ConsoleColor.DarkCyan, $"  > {otherSolution.FilePath}");
                }
            }

            Console.WriteLine();
            Console.WriteLine();

            // Overlap per solution.
            var solutionsWithOverlap = codeBase.Solutions
                .Where(s =>
                    s != solution &&
                    !excludedSolutions.Contains(s))
                .Select(s => new
                {
                    Solution = s,
                    CommonProjects = s.IncludedProjects.Intersect(solution.IncludedProjects).ToList(),
                })
                .OrderByDescending(s => s.CommonProjects.Count)
                .ToList();

            var cumulativeCommonProjects = new List<IProject>();
            ConsoleHelper.WriteLineInColor(ConsoleColor.White, $"Solutions with project overlap:");

            foreach (var solutionWithOverlap in solutionsWithOverlap)
            {
                cumulativeCommonProjects = cumulativeCommonProjects.Union(solutionWithOverlap.CommonProjects).ToList();

                ConsoleHelper.WriteLineInColor(ConsoleColor.Cyan, $"> {solutionWithOverlap.Solution.FilePath}:");
                ConsoleHelper.WriteLineInColor(ConsoleColor.DarkCyan, $"  > Includes {solutionWithOverlap.CommonProjects.Count} common projects(s).");
                ConsoleHelper.WriteLineInColor(ConsoleColor.DarkCyan, $"  > Cumulatively (with previous solutions) covers {cumulativeCommonProjects.Count} of  the {solution.IncludedProjects.Count} projects(s).");
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
