using CodeBaseAnalyzer.Graph.Model.Internal;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Projects
{
    /// <summary>
    /// Abstracts tasks perfomed on .NET project files, to allow for support of different project file formats.
    /// </summary>
    internal interface IProjectTasks
    {
        /// <summary>
        /// Gets all the files included in the project represented by the specified project file.
        /// </summary>
        /// <param name="projectFilePath">
        /// The project file to analyze
        /// </param>
        /// <param name="allSourceCodeFiles">
        /// A dictionary of all known source code files in the code base, organized by file path
        /// </param>
        /// <param name="addIssue">
        /// A delegate to add/register issues which occur while performing the task
        /// </param>
        /// <returns>
        /// All the files included in the project
        /// </returns>
        IEnumerable<SourceCodeFile> GetIncludedFiles(string projectFilePath, IDictionary<string, SourceCodeFile> allSourceCodeFiles, Action<Issue> addIssue);
    }
}
