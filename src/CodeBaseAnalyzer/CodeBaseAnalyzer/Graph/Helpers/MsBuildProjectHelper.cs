using CodeBaseAnalyzer.Base;
using System.Xml;

namespace CodeBaseAnalyzer.Graph.Helpers
{
    public class MsBuildProjectHelper : IMsBuildProjectHelper
    {
        public IReadOnlyList<string> GetProjectReferenceIncludes(XmlDocument msBuildProjectDocument) =>
            GetSpecificElementAttributes(msBuildProjectDocument, "ProjectReference", "Include");

        public IReadOnlyList<string> GetCompileIncludes(XmlDocument msBuildProjectDocument) =>
            GetSpecificElementAttributes(msBuildProjectDocument, "Compile", "Include");

        public IReadOnlyList<string> GetCompileRemoves(XmlDocument msBuildProjectDocument) =>
            GetSpecificElementAttributes(msBuildProjectDocument, "Compile", "Remove");

        private static IReadOnlyList<string> GetSpecificElementAttributes(XmlDocument xmlDocument, string elementTagName, string attributeName)
        {
            Argument.AssertNotNull(xmlDocument, nameof(xmlDocument));
            Argument.AssertNotNull(elementTagName, nameof(elementTagName));
            Argument.AssertNotNull(attributeName, nameof(attributeName));

            var elements = xmlDocument.GetElementsByTagName(elementTagName);
            var attributeValues = elements
                .Cast<XmlNode>()
                .Select(e => e.Attributes
                    .Cast<XmlAttribute>()
                    .SingleOrDefault(a => a.LocalName == attributeName)
                    ?.Value)
                .Where(s => s != null)
                .ToList();

            return attributeValues;
        }
    }
}
