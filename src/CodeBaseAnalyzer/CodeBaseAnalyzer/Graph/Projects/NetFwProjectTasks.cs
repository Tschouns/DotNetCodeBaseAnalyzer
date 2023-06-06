using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Graph.Helpers;
using CodeBaseAnalyzer.Graph.Model.Internal;
using CodeBaseAnalyzer.Issues;
using System.Xml;

namespace CodeBaseAnalyzer.Graph.Projects
{
    /// <summary>
    /// Project tasks implementation for old-school .NET Framework (4.x, and earlier) projects.
    /// </summary>
    internal class NetFwProjectTasks : IProjectTasks
    {
        private readonly IMsBuildProjectHelper msBuildProjectHelper;

        public NetFwProjectTasks(IMsBuildProjectHelper msBuildProjectHelper)
        {
            Argument.AssertNotNull(msBuildProjectHelper, nameof(msBuildProjectHelper));

            this.msBuildProjectHelper = msBuildProjectHelper;
        }

        public IEnumerable<SourceCodeFile> GetIncludedFiles(string projectFilePath, IDictionary<string, SourceCodeFile> allSourceCodeFiles, Action<Issue> addIssue)
        {
            Argument.AssertNotNull(projectFilePath, nameof(projectFilePath));
            Argument.AssertNotNull(allSourceCodeFiles, nameof(allSourceCodeFiles));
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

                return new SourceCodeFile[0];
            }

            var compileIncludes = this.msBuildProjectHelper.GetCompileIncludes(projectFile);
            var projectBaseDirectory = Path.GetDirectoryName(projectFilePath);
            var includedSourceCodeFiles = new List<SourceCodeFile>();

            foreach (var compileInclude in compileIncludes)
            {
                var includedFileAbsolutePath = PathHelper.CombineToAbsolutePath(projectBaseDirectory, compileInclude);
                if (allSourceCodeFiles.TryGetValue(includedFileAbsolutePath, out var sourceCodeFile))
                {
                    includedSourceCodeFiles.Add(sourceCodeFile);
                }
            }

            return includedSourceCodeFiles;
        }
    }
}
