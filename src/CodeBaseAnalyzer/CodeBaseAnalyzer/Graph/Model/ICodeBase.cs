using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Model
{
    /// <summary>
    /// Represents the entire analyzed .NET code base within a root node.
    /// </summary>
    public interface ICodeBase
    {
        /// <summary>
        /// Gets the code base root directory.
        /// </summary>
        public string RootDirectory { get; }

        /// <summary>
        /// Gets all the solutions in the code base directory.
        /// </summary>
        public IReadOnlyList<ISolution> Solutions { get; }

        /// <summary>
        /// Gets all the projects in the code base directory.
        /// </summary>
        public IReadOnlyList<IProject> Projects { get; }

        /// <summary>
        /// Gets all the source code files in the code basedirectory.
        /// </summary>
        public IReadOnlyList<ISourceCodeFile> SourceCodeFiles { get; }

        /// <summary>
        /// Gets the issues which occurred while generating the graph.
        /// </summary>
        public IReadOnlyList<Issue> Issues { get; }
    }
}
