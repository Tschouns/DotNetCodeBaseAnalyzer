using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Model.Internal
{
    internal class CodeBase : ICodeBase
    {
        private readonly Lazy<IReadOnlyList<ISolution>> lazyOrderedSolutions;
        private readonly Lazy<IReadOnlyList<IProject>> lazyOrderedProjects;
        private readonly Lazy<IReadOnlyList<ISourceCodeFile>> lazyOrderedSourceCodeFiles;

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
            this.Issues = issues;

            this.lazyOrderedSolutions = new Lazy<IReadOnlyList<ISolution>>(() => solutions.OrderBy(x => x.FilePath).ToList());
            this.lazyOrderedProjects = new Lazy<IReadOnlyList<IProject>>(() => projects.OrderBy(x => x.FilePath).ToList());
            this.lazyOrderedSourceCodeFiles = new Lazy<IReadOnlyList<ISourceCodeFile>>(() => sourceCodeFiles.OrderBy(x => x.FilePath).ToList());
        }

        public string RootDirectory { get; }

        public IReadOnlyList<ISolution> Solutions => this.lazyOrderedSolutions.Value;

        public IReadOnlyList<IProject> Projects => this.lazyOrderedProjects.Value;

        public IReadOnlyList<ISourceCodeFile> SourceCodeFiles => this.lazyOrderedSourceCodeFiles.Value;

        public IReadOnlyList<Issue> Issues { get; }
    }
}
