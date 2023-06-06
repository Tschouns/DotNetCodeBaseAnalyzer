using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Model.Internal
{
    internal class Solution : ISolution
    {
        public Solution(string filePath)
        {
            Argument.AssertNotNull(filePath, nameof(filePath));

            this.FilePath = filePath;
        }

        public string FilePath { get; }
        public IReadOnlyList<IProject> IncludedProjects => this.IncludedProjectsInternal;
        public IReadOnlyList<Issue> Issues => this.IssuesInternal;

        internal List<Project> IncludedProjectsInternal { get; } = new List<Project>();
        internal List<Issue> IssuesInternal { get; } = new List<Issue>();

        public override string ToString()
        {
            return this.FilePath;
        }
    }
}
