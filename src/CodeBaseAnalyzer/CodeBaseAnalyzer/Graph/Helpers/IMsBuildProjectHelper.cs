
using System.Xml;

namespace CodeBaseAnalyzer.Graph.Helpers
{
    /// <summary>
    /// Helps with analyzing MS Build project (XML) files.
    /// </summary>
    public interface IMsBuildProjectHelper
    {
        /// <summary>
        /// Gets the values of "Include" attributes of all the "ProjectReference" elements.
        /// </summary>
        /// <param name="msBuildProjectDocument">
        /// The MS build project file as XML document
        /// </param>
        /// <returns>
        /// A list of all the attribute values
        /// </returns>
        IReadOnlyList<string> GetProjectReferenceIncludes(XmlDocument msBuildProjectDocument);

        /// <summary>
        /// Gets the values of "Include" attributes of all the "Compile" elements.
        /// </summary>
        /// <param name="msBuildProjectDocument">
        /// The MS build project file as XML document
        /// </param>
        /// <returns>
        /// A list of all the attribute values
        /// </returns>
        IReadOnlyList<string> GetCompileIncludes(XmlDocument msBuildProjectDocument);

        /// <summary>
        /// Gets the values of "Remove" attributes of all the "Compile" elements.
        /// </summary>
        /// <param name="msBuildProjectDocument">
        /// The MS build project file as XML document
        /// </param>
        /// <returns>
        /// A list of all the attribute values
        /// </returns>
        IReadOnlyList<string> GetCompileRemoves(XmlDocument msBuildProjectDocument);
    }
}
