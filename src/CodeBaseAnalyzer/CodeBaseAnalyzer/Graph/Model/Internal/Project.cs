using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Issues;
using System.Collections.Generic;

namespace CodeBaseAnalyzer.Graph.Model.Internal
{
    internal class Project : IProject
    {
        private readonly Lazy<IReadOnlyList<ISourceCodeFile>> lazyOrderedSourceCodeFiles;
        private readonly Lazy<IReadOnlyList<IProject>> lazyOrderedReferencedProjects;
        private readonly Lazy<IReadOnlyList<IProject>> lazyOrderedDependentProjects;
        private readonly Lazy<IReadOnlyList<ISolution>> lazyOrderedDependentSolutions;

        public Project(string filePath)
        {
            Argument.AssertNotNull(filePath, nameof(filePath));

            this.FilePath = filePath;
            this.lazyOrderedSourceCodeFiles = new Lazy<IReadOnlyList<ISourceCodeFile>>(() => this.SourceCodeFilesInternal.OrderBy(x => x.FilePath).ToList());
            this.lazyOrderedReferencedProjects = new Lazy<IReadOnlyList<IProject>>(() => this.ReferencedProjectsInternal.OrderBy(x => x.FilePath).ToList());
            this.lazyOrderedDependentProjects = new Lazy<IReadOnlyList<IProject>>(() => this.DependentProjectsInternal.OrderBy(x => x.FilePath).ToList());
            this.lazyOrderedDependentSolutions = new Lazy<IReadOnlyList<ISolution>>(() => this.DependentSolutionsInternal.OrderBy(x => x.FilePath).ToList());
        }

        public string FilePath { get; }
        public IReadOnlyList<ISourceCodeFile> SourceCodeFiles => this.lazyOrderedSourceCodeFiles.Value;
        public IReadOnlyList<IProject> ReferencedProjects => this.lazyOrderedReferencedProjects.Value;
        public IReadOnlyList<IProject> DependentProjects => this.lazyOrderedDependentProjects.Value;
        public IReadOnlyList<ISolution> DependentSolutions => this.lazyOrderedDependentSolutions.Value;
        public IReadOnlyList<Issue> Issues => this.IssuesInternal;

        internal List<SourceCodeFile> SourceCodeFilesInternal { get; } = new List<SourceCodeFile>();
        internal List<Project> ReferencedProjectsInternal { get; } = new List<Project>();
        internal List<Project> DependentProjectsInternal { get; } = new List<Project>();
        internal List<Solution> DependentSolutionsInternal { get; } = new List<Solution>();
        internal List<Issue> IssuesInternal { get; } = new List<Issue>();

        public override string ToString()
        {
            return this.FilePath;
        }
    }
}
