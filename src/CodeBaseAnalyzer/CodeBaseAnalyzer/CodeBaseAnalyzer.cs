
using CodeBaseAnalyzer.Graph;
using CodeBaseAnalyzer.Graph.Helpers;
using CodeBaseAnalyzer.Search;

namespace CodeBaseAnalyzer
{
    /// <summary>
    /// Provides tools to analyze a code base.
    /// </summary>
    public static class CodeBaseAnalyzer
    {
        /// <summary>
        /// Gets the search helper: helps with searching for C#.NET solution, project, and source code files.
        /// </summary>
        public static ISearchHelper Search { get; } = new SearchHelper();

        /// <summary>
        /// Gets the code base graph generator: generates a graph representation of a code base.
        /// </summary>
        public static ICodeBaseGraphGenerator Graph { get; } = new CodeBaseGraphGenerator();
    }
}
