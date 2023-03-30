using CodeBaseAnalyzer.Graph.Helpers;
using CodeBaseAnalyzer.Tests.TestData;
using System.Xml;

namespace CodeBaseAnalyzer.Tests.Graph.Helpers
{
    public class MsBuildProjectHelperTests
    {
        [Theory]
        [InlineData("Base_Common_csproj", 1)]
        [InlineData("Base_CommonCP_csproj", 1)]
        [InlineData("Base_CommonDesktop_csproj", 1)]
        [InlineData("Base_CommonDesktop_Net6_csproj", 1)]
        public void GetProjectReferenceIncludes_FromExampleProjectFile_ReturnsExpectedNumberOfProjectReferenceIncludes(string projectFileName, int expectedNumberOfProjectReferenceIncludes)
        {
            // Arrange
            var testee = new MsBuildProjectHelper();

            var projectFileText = Files.ResourceManager.GetString(projectFileName);
            var projectXmlDocument = new XmlDocument();
            projectXmlDocument.LoadXml(projectFileText);

            // Act
            var actualProjectReferenceIncludes = testee.GetProjectReferenceIncludes(projectXmlDocument);

            // Assert
            Assert.Equal(expectedNumberOfProjectReferenceIncludes, actualProjectReferenceIncludes.Count);
        }

        [Theory]
        [InlineData("Base_Common_csproj", 242)]
        [InlineData("Base_CommonCP_csproj", 0)]
        [InlineData("Base_CommonDesktop_csproj", 237)]
        [InlineData("Base_CommonDesktop_Net6_csproj", 0)]
        public void GetCompileIncludes_FromExampleProjectFile_ReturnsExpectedNumberOfCompileIncludes(string projectFileName, int expectedNumberOfCompileIncludes)
        {
            // Arrange
            var testee = new MsBuildProjectHelper();

            var projectFileText = Files.ResourceManager.GetString(projectFileName);
            var projectXmlDocument = new XmlDocument();
            projectXmlDocument.LoadXml(projectFileText);

            // Act
            var actualCompileIncludes = testee.GetCompileIncludes(projectXmlDocument);

            // Assert
            Assert.Equal(expectedNumberOfCompileIncludes, actualCompileIncludes.Count);
        }

        [Theory]
        [InlineData("Base_Common_csproj", 0)]
        [InlineData("Base_CommonCP_csproj", 5)]
        [InlineData("Base_CommonDesktop_csproj", 0)]
        [InlineData("Base_CommonDesktop_Net6_csproj", 2)]
        public void GetCompileRemoves_FromExampleProjectFile_ReturnsExpectedNumberOfCompileRemoves(string projectFileName, int expectedNumberOfCompileRemoves)
        {
            // Arrange
            var testee = new MsBuildProjectHelper();

            var projectFileText = Files.ResourceManager.GetString(projectFileName);
            var projectXmlDocument = new XmlDocument();
            projectXmlDocument.LoadXml(projectFileText);

            // Act
            var actualCompileRemoves = testee.GetCompileRemoves(projectXmlDocument);

            // Assert
            Assert.Equal(expectedNumberOfCompileRemoves, actualCompileRemoves.Count);
        }
    }
}
