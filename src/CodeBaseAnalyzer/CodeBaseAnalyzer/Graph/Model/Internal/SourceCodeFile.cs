
namespace CodeBaseAnalyzer.Graph.Model.Internal
{
    internal class SourceCodeFile : ISourceCodeFile
    {
        public SourceCodeFile(string filePath)
        {
            this.FilePath = filePath;
        }

        public string FilePath { get; }
        public IReadOnlyList<IProject> DependentProjects => this.DependentProjectsInternal;

        internal List<Project> DependentProjectsInternal { get; } = new List<Project>();

        public override string ToString()
        {
            return this.FilePath;
        }
    }
}
