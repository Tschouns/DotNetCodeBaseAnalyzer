using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Graph.Helpers;
using CodeBaseAnalyzer.Graph.Model;
using CodeBaseAnalyzer.Graph.Model.Internal;
using CodeBaseAnalyzer.Graph.Projects;
using CodeBaseAnalyzer.Issues;
using CodeBaseAnalyzer.Search;
using Microsoft.Build.Construction;
using Microsoft.Build.Exceptions;
using System.Xml;

namespace CodeBaseAnalyzer.Graph
{
    public class CodeBaseGraphGenerator : ICodeBaseGraphGenerator
    {
        private readonly ISearchHelper searchHelper;
        private readonly IMsBuildProjectHelper msBuildProjectHelper;

        public CodeBaseGraphGenerator()
            : this(new SearchHelper(), new MsBuildProjectHelper())
        {
        }

        public CodeBaseGraphGenerator(ISearchHelper searchHelper, IMsBuildProjectHelper msBuildProjectHelper)
        {
            Argument.AssertNotNull(searchHelper, nameof(searchHelper));
            Argument.AssertNotNull(msBuildProjectHelper, nameof(msBuildProjectHelper));

            this.searchHelper = searchHelper;
            this.msBuildProjectHelper = msBuildProjectHelper;
        }

        public ICodeBase GenerateGraph(string codeBaseRootDirectory)
        {
            Argument.AssertNotNull(codeBaseRootDirectory, nameof(codeBaseRootDirectory));

            if (!Directory.Exists(codeBaseRootDirectory))
            {
                throw new ArgumentException($"The root directory \"{codeBaseRootDirectory}\" does not exist.", nameof(codeBaseRootDirectory));
            }

            var codeBaseRootDirectoryAbsolute = PathHelper.CombineToAbsolutePath(Environment.CurrentDirectory, codeBaseRootDirectory);

            var solutionFilePaths = this.searchHelper.FindFilteredAndOrdered(codeBaseRootDirectoryAbsolute, "*.sln");
            var projectFilePaths = this.searchHelper.FindFilteredAndOrdered(codeBaseRootDirectoryAbsolute, "*.csproj");
            var sourceCodeFilePaths = this.searchHelper.FindFilteredAndOrdered(codeBaseRootDirectoryAbsolute, "*.cs");

            var solutions = solutionFilePaths.Select(s => new Solution(s)).ToList();
            var projects = projectFilePaths.Select(p => new Project(p)).ToList();
            var sourceCodeFiles = sourceCodeFilePaths.Select(c => new SourceCodeFile(c)).ToList();

            var allIssues = new List<Issue>();

            foreach (var solution in solutions)
            {
                Action<Issue> addIssue = i =>
                {
                    // Add to "all".
                    allIssues.Add(i);

                    // Add to solution.
                    solution.IssuesInternal.Add(i);
                };

                this.AmendSolutionProjectRelationship(solution, projects, addIssue);
            }

            foreach (var project in projects)
            {
                var dependentSolutions = solutions.Where(s => project.DependentSolutions.Contains(s)).ToList();

                Action<Issue> addIssue = i =>
                {
                    // Add to "all".
                    allIssues.Add(i);

                    // Add to project.
                    project.IssuesInternal.Add(i);

                    // Add to each solution.
                    foreach (var solution in dependentSolutions)
                    {
                        solution.IssuesInternal.Add(i);
                    }
                };

                this.AmendProjectProjectRelationship(project, projects, addIssue);
                this.AmendProjectSourceCodeRelationship(project, sourceCodeFiles, addIssue);
            }

            var codeBase = new CodeBase(codeBaseRootDirectoryAbsolute, solutions, projects, sourceCodeFiles, allIssues);

            return codeBase;
        }

        private void AmendSolutionProjectRelationship(Solution solution, IReadOnlyList<Project> allProjects, Action<Issue> addIssue)
        {
            Argument.AssertNotNull(solution, nameof(solution));
            Argument.AssertNotNull(allProjects, nameof(allProjects));
            Argument.AssertNotNull(addIssue, nameof(addIssue));

            try
            {
                var solutionFile = SolutionFile.Parse(solution.FilePath);

                foreach (var projectReference in solutionFile.ProjectsInOrder.Where(p => p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat))
                {
                    var project = allProjects.SingleOrDefault(p => p.FilePath == projectReference.AbsolutePath);

                    if (project == null)
                    {
                        addIssue(Issue.Error($"The solution \"{solution.FilePath}\" references a project \"{projectReference.AbsolutePath}\" which does not exist."));
                    }
                    else
                    {
                        // Associate solution and project.
                        solution.IncludedProjectsInternal.Add(project);
                        project.DependentSolutionsInternal.Add(solution);
                    }
                }
            }
            catch (InvalidProjectFileException ex)
            {
                addIssue(Issue.Warn($"The solution file \"{solution.FilePath}\" could not be parsed: {ex}"));

                return;
            }
        }

        private void AmendProjectProjectRelationship(Project project, IReadOnlyList<Project> allProjects, Action<Issue> addIssue)
        {
            Argument.AssertNotNull(project, nameof(project));
            Argument.AssertNotNull(allProjects, nameof(allProjects));
            Argument.AssertNotNull(addIssue, nameof(addIssue));

            var projectFile = new XmlDocument();

            try
            {
                projectFile.Load(project.FilePath);
            }
            catch (XmlException ex)
            {
                addIssue(Issue.Warn($"The project file \"{project.FilePath}\" could not be parsed: {ex}"));

                return;
            }

            var projectReferences = this.msBuildProjectHelper.GetProjectReferenceIncludes(projectFile);
            var projectBaseDirectory = Path.GetDirectoryName(project.FilePath);

            foreach (var projectReference in projectReferences)
            {
                // Determine the absolute path of the referenced project.
                var projectReferenceAbsolute = PathHelper.CombineToAbsolutePath(projectBaseDirectory, projectReference);
                var referencedProject = allProjects.SingleOrDefault(p => p.FilePath == projectReferenceAbsolute);

                if (referencedProject == null)
                {
                    addIssue(Issue.Error($"The project \"{project.FilePath}\" references another project \"{projectReferenceAbsolute}\" which does not exist."));
                }
                else
                {
                    // Associate the two projects.
                    project.ReferencedProjectsInternal.Add(referencedProject);
                    referencedProject.DependentProjectsInternal.Add(project);
                }
            }
        }

        private void AmendProjectSourceCodeRelationship(Project project, IReadOnlyList<SourceCodeFile> allSourceCodeFile, Action<Issue> addIssue)
        {
            Argument.AssertNotNull(project, nameof(project));
            Argument.AssertNotNull(allSourceCodeFile, nameof(allSourceCodeFile));
            Argument.AssertNotNull(addIssue, nameof(addIssue));

            var projectTasks = this.SelectProjectTasks(project.FilePath, addIssue);

            var allIncludedFilePaths = projectTasks.GetIncludedFiles(project.FilePath, allSourceCodeFile, addIssue);
            var projectBaseDirectory = Path.GetDirectoryName(project.FilePath);

            // TODO: do this filtering stuff inside the IProjectTasks implementation, based on "all source code files" list.
            // Determine which are actually source code.
            foreach (var includedFilePath in allIncludedFilePaths)
            {
                // Determine the absolute path of the referenced file.
                var includedFileAbsolutePaths = PathHelper.CombineToAbsolutePath(projectBaseDirectory, includedFilePath);
                var referencedSourceCodeFile = allSourceCodeFile.SingleOrDefault(c => c.FilePath == includedFileAbsolutePaths);

                if (referencedSourceCodeFile != null)
                {
                    // Associate the project and source code file.
                    project.SourceCodeFilesInternal.Add(referencedSourceCodeFile);
                    referencedSourceCodeFile.DependentProjectsInternal.Add(project);
                }
            }
        }

        private IProjectTasks SelectProjectTasks(string projectFilePath, Action<Issue> addIssue)
        {
            Argument.AssertNotNull(projectFilePath, nameof(projectFilePath));
            Argument.AssertNotNull(addIssue, nameof(addIssue));

            if (!File.Exists(projectFilePath))
            {
                throw new ArgumentException($"The specified project file \"{projectFilePath}\" was not found.", nameof(projectFilePath));
            }

            // Load as XML doc.
            var projectFile = new XmlDocument();

            try
            {
                projectFile.Load(projectFilePath);
            }
            catch (XmlException ex)
            {
                addIssue(Issue.Warn($"The project file \"{projectFile}\" could not be parsed: {ex}"));

                return new DummyProjectTasks();
            }

            // TOOD: Is this realy the smartest way to determine the project file type?
            var compileIncludes = this.msBuildProjectHelper.GetCompileIncludes(projectFile);
            var compileRemoves = this.msBuildProjectHelper.GetCompileRemoves(projectFile);

            if (compileRemoves.Any())
            {
                // Strange...
                ////addIssue(Issue.Warn($"The project file format for project file \"{projectFilePath}\" could not be determined."));

                // .NET Core and newer (.NET5, 6, etc.).
                return new NetCoreProjectTasks(this.msBuildProjectHelper);
            }

            if (compileIncludes.Any())
            {


                // Old-school .NET Framework (4.x, etc.).
                return new NetFwProjectTasks(this.msBuildProjectHelper);
            }

            // When in doubt... .NET Core and newer (.NET5, 6, etc.).
            return new NetCoreProjectTasks(this.msBuildProjectHelper);
        }
    }
}
