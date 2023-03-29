using CodeBaseAnalyzer.Graph;

namespace CodeBaseAnalyzer.Tests.Graph
{
    public class CodeBaseGraphGeneratorTests
    {
        [Fact]
        public void GenerateGraph_CountFileTypes_FindsExpectedNumbers()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Assert.Equal(TestEnvironment.ExpectedNumberOfSolutions, codeBase.Solutions.Count());
            Assert.Equal(TestEnvironment.ExpectedNumberOfProjects, codeBase.Projects.Count());
            Assert.Equal(TestEnvironment.ExpectedNumberOfCodeFiles, codeBase.SourceCodeFiles.Count());
        }

        [Fact]
        public void GenerateGraph_GetFilePaths_AreAbsolutePaths()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Array.ForEach(codeBase.Solutions.ToArray(), s => Assert.True(Path.IsPathRooted(s.FilePath)));
            Array.ForEach(codeBase.Projects.ToArray(), p => Assert.True(Path.IsPathRooted(p.FilePath)));
            Array.ForEach(codeBase.SourceCodeFiles.ToArray(), c => Assert.True(Path.IsPathRooted(c.FilePath)));
        }

        [Fact]
        public void GenerateGraph_SolutionProjects_FindsAtLeastOneProjectPerSolutions()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Assert.All(codeBase.Solutions, s => s.IncludedProjects.Any());
        }

        [Fact]
        public void GenerateGraph_SolutionProjects_AreAllContainedInCodeBaseProjects()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Array.ForEach(
                codeBase.Solutions.ToArray(),
                s => Assert.True(s.IncludedProjects.All(p => codeBase.Projects.Contains(p))));
        }

        [Fact]
        public void GenerateGraph_SolutionProjects_AllReverseReferenceSolution()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Array.ForEach(
                codeBase.Solutions.ToArray(),
                s => Assert.True(s.IncludedProjects.All(p => p.DependentSolutions.Contains(s))));
        }

        [Fact]
        public void GenerateGraph_ProjectReferencedProjects_AllReverseReferenceDependentProject()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Array.ForEach(
                codeBase.Projects.ToArray(),
                p => Assert.True(p.ReferencedProjects.All(r => r.DependentProjects.Contains(p))));
        }

        [Fact]
        public void GenerateGraph_ProjectDependentProjects_AllReferenceReferencedProject()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Array.ForEach(
                codeBase.Projects.ToArray(),
                p => Assert.True(p.DependentProjects.All(d => d.ReferencedProjects.Contains(p))));
        }

        [Fact]
        public void GenerateGraph_ProjectSourceCodeFiles_AreAllContainedInCodeBaseSourceCodeFiles()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Array.ForEach(
                codeBase.Projects.ToArray(),
                p => Assert.True(p.SourceCodeFiles.All(c => codeBase.SourceCodeFiles.Contains(c))));
        }

        [Fact]
        public void GenerateGraph_ProjectSourceCodeFiles_FindsAtLeastOneCodeFilePerProject()
        {
            // Arrange
            var testee = new CodeBaseGraphGenerator();

            // Act
            var codeBase = testee.GenerateGraph(TestEnvironment.TestCodeBaseDirectory);

            // Assert
            Assert.All(codeBase.Projects, s => s.SourceCodeFiles.Any());
        }
    }
}