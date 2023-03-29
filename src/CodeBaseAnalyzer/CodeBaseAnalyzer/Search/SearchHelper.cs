using CodeBaseAnalyzer.Base;

namespace CodeBaseAnalyzer.Search
{
    public class SearchHelper : ISearchHelper
    {
        public IEnumerable<string> FindSolutionFiles(string directory)
        {
            Argument.AssertNotNull(directory, nameof(directory));

            var filter = "*.sln";

            return this.FindFilteredAndOrdered(directory, filter);
        }

        public IEnumerable<string> FindProjectFiles(string directory)
        {
            Argument.AssertNotNull(directory, nameof(directory));

            var filter = "*.csproj";

            return this.FindFilteredAndOrdered(directory, filter);
        }

        public IEnumerable<string> FindCodeFiles(string directory)
        {
            Argument.AssertNotNull(directory, nameof(directory));

            var filter = "*.cs";

            return this.FindFilteredAndOrdered(directory, filter);
        }

        public IEnumerable<string> FindFilteredAndOrdered(string directory, string filter)
        {
            Argument.AssertNotNull(directory, nameof(directory));
            Argument.AssertNotNull(filter, nameof(filter));

            var files = Directory.EnumerateFiles(directory, filter, SearchOption.AllDirectories);
            var orderedFiles = files.OrderBy(f => f).ToList();

            return orderedFiles;
        }
    }
}