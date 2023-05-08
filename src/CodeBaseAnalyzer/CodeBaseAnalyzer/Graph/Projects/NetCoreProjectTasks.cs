using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Graph.Helpers;
using CodeBaseAnalyzer.Graph.Model;
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

        public IEnumerable<string> GetIncludedFiles(string projectFilePath, IReadOnlyList<ISourceCodeFile> allSourceCodeFiles, Action<Issue> addIssue)
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

                return new string[0];
            }

            var projectBaseDirectory = Path.GetDirectoryName(projectFilePath);

            // Find (implicitly included) files within project directory.
            var implicitlyIncludedFiles = allSourceCodeFiles
                .Select(c => c.FilePath)
                .Where(p => p.StartsWith(projectBaseDirectory))
                .ToList();

            // Determine which files are explicitly expluded in the project file.
            var compileRemoves = this.msBuildProjectHelper.GetCompileRemoves(projectFile);
            var compileRemovesAbsolute = compileRemoves
                .Select(p => PathHelper.CombineToAbsolutePath(projectBaseDirectory, p))
                .ToList();

            // Filter files.
            var remainingFiles = implicitlyIncludedFiles
                .Where(i => !compileRemovesAbsolute.Contains(i))
                .ToList();

            return remainingFiles;
        }
    }
}
