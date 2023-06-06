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

            // Pre-ordered files (not strictly necessary, but beneficial performancewise).
            var solutionFilePaths = this.searchHelper.FindFilteredAndOrdered(codeBaseRootDirectoryAbsolute, "*.sln");
            var projectFilePaths = this.searchHelper.FindFilteredAndOrdered(codeBaseRootDirectoryAbsolute, "*.csproj");
            var sourceCodeFilePaths = this.searchHelper.FindFilteredAndOrdered(codeBaseRootDirectoryAbsolute, "*.cs");

            var solutions = solutionFilePaths.Select(s => new Solution(s)).ToList();
            var projects = projectFilePaths.Select(p => new Project(p)).ToList();
            var sourceCodeFiles = sourceCodeFilePaths.Select(c => new SourceCodeFile(c)).ToList();

            var solutionsDict = solutions.ToDictionary(s => s.FilePath);
            var projectsDict = projects.ToDictionary(p => p.FilePath);
            var sourceCodeFilesDict = sourceCodeFiles.ToDictionary(c => c.FilePath);

            var allIssues = new List<Issue>();

            // Process all solutions async.
            var solutionTasks = solutions
                .Select(solution => new Task(() =>
                    {
                        Action<Issue> addIssue = i =>
                        {
                            // Add to "all".
                            allIssues.Add(i);

                            // Add to solution.
                            solution.IssuesInternal.Add(i);
                        };

                        this.AmendSolutionProjectRelationship(solution, projectsDict, addIssue);
                    }))
                .ToArray();

            foreach(var task in solutionTasks)
            {
                task.Start();
            }

            Task.WaitAll(solutionTasks);

            // Process all projects async.
            var projectTasks = projects
                .Select(project => new Task(() =>
                    {
                        Action<Issue> addIssue = i =>
                        {
                            // Add to "all".
                            allIssues.Add(i);

                            // Add to project.
                            project.IssuesInternal.Add(i);

                            // Add to each solution.
                            foreach (var solution in project.DependentSolutionsInternal)
                            {
                                solution.IssuesInternal.Add(i);
                            }
                        };

                        this.AmendProjectProjectRelationship(project, projectsDict, addIssue);
                        this.AmendProjectSourceCodeRelationship(project, sourceCodeFilesDict, addIssue);
                    }))
                .ToArray();

            foreach (var task in projectTasks)
            {
                task.Start();
            }

            Task.WaitAll(projectTasks);

            // Assemble the "code base" model.
            var codeBase = new CodeBase(codeBaseRootDirectoryAbsolute, solutions, projects, sourceCodeFiles, allIssues);

            return codeBase;
        }

        private void AmendSolutionProjectRelationship(Solution solution, IDictionary<string, Project> allProjects, Action<Issue> addIssue)
        {
            Argument.AssertNotNull(solution, nameof(solution));
            Argument.AssertNotNull(allProjects, nameof(allProjects));
            Argument.AssertNotNull(addIssue, nameof(addIssue));

            try
            {
                var solutionFile = SolutionFile.Parse(solution.FilePath);

                foreach (var projectReference in solutionFile.ProjectsInOrder.Where(p => p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat))
                {
                    if (allProjects.TryGetValue(projectReference.AbsolutePath, out var project))
                    {
                        // Associate solution and project.
                        solution.IncludedProjectsInternal.Add(project);

                        lock (project.DependentSolutionsInternal)
                        {
                            project.DependentSolutionsInternal.Add(solution);
                        }
                    }
                    else
                    {
                        addIssue(Issue.Error($"The solution \"{solution.FilePath}\" references a project \"{projectReference.AbsolutePath}\" which does not exist."));
                    }
                }
            }
            catch (InvalidProjectFileException ex)
            {
                addIssue(Issue.Warn($"The solution file \"{solution.FilePath}\" could not be parsed: {ex}"));

                return;
            }
        }

        private void AmendProjectProjectRelationship(Project project, IDictionary<string, Project> allProjects, Action<Issue> addIssue)
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

                if (allProjects.TryGetValue(projectReferenceAbsolute, out var referencedProject))
                {
                    // Associate the two projects.
                    project.ReferencedProjectsInternal.Add(referencedProject);

                    lock (referencedProject.DependentProjectsInternal)
                    {
                        referencedProject.DependentProjectsInternal.Add(project);
                    }
                }
                else
                {
                    addIssue(Issue.Error($"The project \"{project.FilePath}\" references another project \"{projectReferenceAbsolute}\" which does not exist."));
                }
            }
        }

        private void AmendProjectSourceCodeRelationship(Project project, IDictionary<string, SourceCodeFile> allSourceCodeFile, Action<Issue> addIssue)
        {
            Argument.AssertNotNull(project, nameof(project));
            Argument.AssertNotNull(allSourceCodeFile, nameof(allSourceCodeFile));
            Argument.AssertNotNull(addIssue, nameof(addIssue));

            var projectTasks = this.SelectProjectTasks(project.FilePath, addIssue);
            var referencedSourceCodeFiles = projectTasks.GetIncludedFiles(project.FilePath, allSourceCodeFile, addIssue);

            foreach (var referencedSourceCodeFile in referencedSourceCodeFiles)
            {
                // Associate the project and source code file.
                project.SourceCodeFilesInternal.Add(referencedSourceCodeFile);

                lock (referencedSourceCodeFile.DependentProjectsInternal)
                {
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
