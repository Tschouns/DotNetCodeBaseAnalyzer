using CodeBaseAnalyzer.Base;
using CodeBaseAnalyzer.Graph.Helpers;
using CodeBaseAnalyzer.Graph.Model;
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

        public IEnumerable<string> GetIncludedFiles(string projectFilePath, IReadOnlyList<ISourceCodeFile> allSourceCodeFiles, Action<Issue> addIssue)
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

                return new string[0];
            }

            var compileIncludes = this.msBuildProjectHelper.GetCompileIncludes(projectFile);

            return compileIncludes;
        }
    }
}
