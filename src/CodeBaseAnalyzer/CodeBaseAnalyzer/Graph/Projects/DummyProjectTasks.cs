using CodeBaseAnalyzer.Graph.Model.Internal;
using CodeBaseAnalyzer.Issues;

namespace CodeBaseAnalyzer.Graph.Projects
{
    /// <summary>
    /// Dummy project tasks implementation, which returns default/dummy values.
    /// </summary>
    internal class DummyProjectTasks : IProjectTasks
    {
        public IEnumerable<SourceCodeFile> GetIncludedFiles(string projectFilePath, IDictionary<string, SourceCodeFile> allSourceCodeFiles, Action<Issue> addIssue) => new SourceCodeFile[0];
    }
}
