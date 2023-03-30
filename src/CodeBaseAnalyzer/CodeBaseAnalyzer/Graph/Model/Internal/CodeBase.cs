using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Model.Internal
{
    internal class CodeBase : ICodeBase
    {
        public CodeBase(
            string rootDirectory,
            IReadOnlyList<ISolution> solutions,
            IReadOnlyList<IProject> projects,
            IReadOnlyList<ISourceCodeFile> sourceCodeFiles,
            IReadOnlyList<Issue> issues)
        {
            Argument.AssertNotNull(rootDirectory, nameof(rootDirectory));
            Argument.AssertNotNull(solutions, nameof(solutions));
            Argument.AssertNotNull(projects, nameof(projects));
            Argument.AssertNotNull(sourceCodeFiles, nameof(sourceCodeFiles));
            Argument.AssertNotNull(issues, nameof(issues));

            this.RootDirectory = rootDirectory;
            this.Solutions = solutions;
            this.Projects = projects;
            this.SourceCodeFiles = sourceCodeFiles;
            this.Issues = issues;
        }

        public string RootDirectory { get; }

        public IReadOnlyList<ISolution> Solutions { get; }

        public IReadOnlyList<IProject> Projects { get; }

        public IReadOnlyList<ISourceCodeFile> SourceCodeFiles { get; }

        public IReadOnlyList<Issue> Issues { get; }
    }
}
