using CodeBaseAnalyzer.Base;

namespace CodeBaseAnalyzer.Tests.Base
{
    public class PathHelperTests
    {
        [Theory()]
        [InlineData("C:", "Foo", "C:\\Foo")]
        [InlineData("C:\\", "\\Foo\\", "C:\\Foo")]
        [InlineData("C:", "Foo\\Bar", "C:\\Foo\\Bar")]
        [InlineData("C:\\Foo", "Bar", "C:\\Foo\\Bar")]
        [InlineData("C:\\Foo", "..\\Bar", "C:\\Bar")]
        [InlineData("C:\\Foo\\Bar", "..\\..\\New", "C:\\New")]
        public void CombineToAbsolutePath_GivenValidBaseDirAndRelative_ReturnsExpectedAbsolutePath(
            string baseDirectoryPath,
            string relativePath,
            string expectedCombinedAbsolutePath)
        {
            // Arrange

            // Act
            var actualCombinedAbsolutePath = PathHelper.CombineToAbsolutePath(baseDirectoryPath, relativePath);

            // Assert
            Assert.Equal(expectedCombinedAbsolutePath, actualCombinedAbsolutePath);
        }
    }
}
