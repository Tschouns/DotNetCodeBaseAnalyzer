using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Model.Internal
{
    internal class Project : IProject
    {
        public Project(string filePath)
        {
            Argument.AssertNotNull(filePath, nameof(filePath));

            this.FilePath = filePath;
        }

        public string FilePath { get; }
        public IReadOnlyList<ISourceCodeFile> SourceCodeFiles => this.SourceCodeFilesInternal;
        public IReadOnlyList<IProject> ReferencedProjects => this.ReferencedProjectsInternal;
        public IReadOnlyList<IProject> DependentProjects => this.DependentProjectsInternal;
        public IReadOnlyList<ISolution> DependentSolutions => this.DependentSolutionsInternal;
        public IReadOnlyList<Issue> Issues => this.IssuesInternal;

        internal List<SourceCodeFile> SourceCodeFilesInternal { get; } = new List<SourceCodeFile>();
        internal List<IProject> ReferencedProjectsInternal { get; } = new List<IProject>();
        internal List<IProject> DependentProjectsInternal { get; } = new List<IProject>();
        internal List<ISolution> DependentSolutionsInternal { get; } = new List<ISolution>();
        internal List<Issue> IssuesInternal { get; } = new List<Issue>();

        public override string ToString()
        {
            return this.FilePath;
        }
    }
}
