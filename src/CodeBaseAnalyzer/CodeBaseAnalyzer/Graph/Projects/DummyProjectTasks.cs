using CodeBaseAnalyzer.Graph.Model;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Projects
{
    /// <summary>
    /// Dummy project tasks implementation, which returns default/dummy values.
    /// </summary>
    internal class DummyProjectTasks : IProjectTasks
    {
        public IEnumerable<string> GetIncludedFiles(string projectFilePath, IReadOnlyList<ISourceCodeFile> allSourceCodeFiles, Action<Issue> addIssue) => new string[0];
    }
}
