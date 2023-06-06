using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Graph.Helpers;
using CodeBaseAnalyzer.Graph.Model.Internal;
using CodeBaseAnalyzer.Issues;
using System.Xml;

namespace CodeBaseAnalyzer.Graph.Projects
{
    /// <summary>
    /// Project tasks implementation for .NET Core and newer (.NET5, 6, etc.) projects.
    /// </summary>
    internal class NetCoreProjectTasks : IProjectTasks
    {
        private readonly IMsBuildProjectHelper msBuildProjectHelper;

        public NetCoreProjectTasks(IMsBuildProjectHelper msBuildProjectHelper)
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

            if (!Path.IsPathRooted(projectFilePath))
            {
                throw new ArgumentException($"The specified project file path \"{projectFilePath}\" must be rooted.", nameof(projectFilePath));
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

            var projectBaseDirectory = Path.GetDirectoryName(projectFilePath);

            // Determine which files are explicitly expluded in the project file.
            var compileRemoves = this.msBuildProjectHelper.GetCompileRemoves(projectFile);
            var compileRemovesAbsolute = compileRemoves
                .Select(p => PathHelper.CombineToAbsolutePath(projectBaseDirectory, p))
                .ToList();

            // Find (implicitly included) files within project directory, and filter by "removes".
            var implicitlyIncludedFiles = allSourceCodeFiles
                .Where(c =>
                    c.Key.StartsWith(projectBaseDirectory) &&
                    compileRemovesAbsolute.Contains(c.Key))
                .Select(c => c.Value)
                .ToList();

            return implicitlyIncludedFiles;
        }
    }
}
