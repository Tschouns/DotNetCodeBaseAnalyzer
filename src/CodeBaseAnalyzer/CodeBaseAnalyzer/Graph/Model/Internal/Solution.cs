using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Model.Internal
{
    internal class Solution : ISolution
    {
        private readonly Lazy<IReadOnlyList<IProject>> lazyOrderedIncludedProjects;

        public Solution(string filePath)
        {
            Argument.AssertNotNull(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.lazyOrderedIncludedProjects = new Lazy<IReadOnlyList<IProject>>(() => this.IncludedProjectsInternal.OrderBy(p => p.FilePath).ToList());
        }

        public string FilePath { get; }
        public IReadOnlyList<IProject> IncludedProjects => this.lazyOrderedIncludedProjects.Value;
        public IReadOnlyList<Issue> Issues => this.IssuesInternal;

        internal List<Project> IncludedProjectsInternal { get; } = new List<Project>();
        internal List<Issue> IssuesInternal { get; } = new List<Issue>();

        public override string ToString()
        {
            return this.FilePath;
        }
    }
}
