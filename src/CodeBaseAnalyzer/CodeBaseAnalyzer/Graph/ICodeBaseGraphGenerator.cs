
using CodeBaseAnalyzer.Graph.Model;

namespace CodeBaseAnalyzer.Graph
{
    /// <summary>
    /// Analyzes a code base, and generates a graph representation of that code base.
    /// </summary>
    public interface ICodeBaseGraphGenerator
    {
        /// <summary>
        /// Analyzes the specified code base root directory, and generates a graph representation of that code base.
        /// </summary>
        /// <param name="codeBaseRootDirectory">
        /// The code base root directory
        /// </param>
        /// <returns>
        /// A graph representation of the code base
        /// </returns>
        ICodeBase GenerateGraph(string codeBaseRootDirectory);
    }
}
