namespace CodeBaseAnalyzer.Search
{
    /// <summary>
    /// Helps with searching directories for C#.NET solution, project, and source code files.
    /// </summary>
    public interface ISearchHelper
    {
        /// <summary>
        /// Finds all .NET solution files within a specified directory.
        /// </summary>
        /// <param name="directory">
        /// The search directory
        /// </param>
        /// <returns>
        /// All .NET solution files within the directory
        /// </returns>
        IEnumerable<string> FindSolutionFiles(string directory);

        /// <summary>
        /// Finds all C# project files within a specified directory.
        /// </summary>
        /// <param name="directory">
        /// The search directory
        /// </param>
        /// <returns>
        /// All C# project files within the directory
        /// </returns>
        IEnumerable<string> FindProjectFiles(string directory);

        /// <summary>
        /// Finds all C# code files within a specified directory.
        /// </summary>
        /// <param name="directory">
        /// The search directory
        /// </param>
        /// <returns>
        /// All C# code files within the directory
        /// </returns>
        IEnumerable<string> FindCodeFiles(string directory);

        /// <summary>
        /// Finds all files within the specified directory which match the specified filter expression, and
        /// returns them in alphabetical order.
        /// </summary>
        /// <param name="directory">
        /// The search directory
        /// </param>
        /// <param name="directory">
        /// The filter expression
        /// </param>
        /// <returns>
        /// All files matching the filter, ordered alphabetically
        /// </returns>
        IEnumerable<string> FindFilteredAndOrdered(string directory, string filter);
    }
}
