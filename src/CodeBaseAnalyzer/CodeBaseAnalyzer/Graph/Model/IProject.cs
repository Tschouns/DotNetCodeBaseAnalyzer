using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Model
{
    /// <summary>
    /// Represents a .NET project file, which can include code files, and, in turn, be referenced by
    /// solutions.
    /// </summary>
    public interface IProject
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the code files included in the project.
        /// </summary>
        public IReadOnlyList<ISourceCodeFile> SourceCodeFiles { get; }

        /// <summary>
        /// Gets the projects referenced by this project.
        /// </summary>
        public IReadOnlyList<IProject> ReferencedProjects { get; }

        /// <summary>
        /// Gets the projects which reference this project.
        /// </summary>
        public IReadOnlyList<IProject> DependentProjects { get; }

        /// <summary>
        /// Gets the solutions which reference this project.
        /// </summary>
        public IReadOnlyList<ISolution> DependentSolutions { get; }

        /// <summary>
        /// Gets the issues associated with the project.
        /// </summary>
        public IReadOnlyList<Issue> Issues { get; }
    }
}
