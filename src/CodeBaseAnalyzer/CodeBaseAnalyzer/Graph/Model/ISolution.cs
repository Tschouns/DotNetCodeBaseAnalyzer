using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Model
{
    /// <summary>
    /// Represents a .NET solution file, which can include projects.
    /// </summary>
    public interface ISolution
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the projects included in the solution.
        /// </summary>
        public IReadOnlyList<IProject> IncludedProjects { get; }

        /// <summary>
        /// Gets the issues associated with the solution.
        /// </summary>
        public IReadOnlyList<Issue> Issues { get; }
    }
}
