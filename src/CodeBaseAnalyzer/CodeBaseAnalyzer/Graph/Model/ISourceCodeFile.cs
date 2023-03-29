namespace CodeBaseAnalyzer.Graph.Model
{
    /// <summary>
    /// Represents a file which contains the actual .NET source code .
    /// </summary>
    public interface ISourceCodeFile
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the project which include this code file.
        /// </summary>
        public IReadOnlyList<IProject> DependentProjects { get; }
    }
}
