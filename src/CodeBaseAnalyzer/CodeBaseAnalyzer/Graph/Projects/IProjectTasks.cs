using CodeBaseAnalyzer.Graph.Model;
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
        /// A list of all known source code files in the code base
        /// </param>
        /// <param name="addIssue">
        /// A delegate to add/register issues which occur while performing the task
        /// </param>
        /// <returns>
        /// All the files included in the project, as absolute or relative path
        /// </returns>
        IEnumerable<string> GetIncludedFiles(string projectFilePath, IReadOnlyList<ISourceCodeFile> allSourceCodeFiles, Action<Issue> addIssue);
    }
}
