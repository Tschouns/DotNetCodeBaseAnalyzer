
namespace CodeBaseAnalyzer.Graph.Model.Internal
{
    internal class SourceCodeFile : ISourceCodeFile
    {
        private readonly Lazy<IReadOnlyList<IProject>> lazyOrderedDependentProjects;

        public SourceCodeFile(string filePath)
        {
            this.FilePath = filePath;
            this.lazyOrderedDependentProjects = new Lazy<IReadOnlyList<IProject>>(() => this.DependentProjectsInternal.OrderBy(p => p.FilePath).ToList());
        }

        public string FilePath { get; }
        public IReadOnlyList<IProject> DependentProjects => this.lazyOrderedDependentProjects.Value;

        internal List<Project> DependentProjectsInternal { get; } = new List<Project>();

        public override string ToString()
        {
            return this.FilePath;
        }
    }
}
