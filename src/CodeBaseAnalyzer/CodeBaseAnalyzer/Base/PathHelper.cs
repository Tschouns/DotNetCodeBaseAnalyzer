
namespace CodeBaseAnalyzer.Base
{
    /// <summary>
    /// Provides helper methods to handle paths.
    /// </summary>
    public static class PathHelper
    {
        public static string CombineToAbsolutePath(string baseDirectoryPath, string relativePath)
        {
            Argument.AssertNotNull(baseDirectoryPath, nameof(baseDirectoryPath));
            Argument.AssertNotNull(relativePath, nameof(relativePath));

            if (!Path.IsPathRooted(baseDirectoryPath))
            {
                throw new ArgumentException($"The base directory path (\"{baseDirectoryPath}\") must be rooted.", nameof(baseDirectoryPath));
            }

            var baseDirectoryPathElements = baseDirectoryPath.Split('/', '\\').ToList();
            var relativePathElements = relativePath.Split('/', '\\').ToList();

            // Reduce ".." back-steps.
            while (
                relativePathElements.Any() &&
                relativePathElements.First().Trim() == "..")
            {
                if (baseDirectoryPathElements.Any() && baseDirectoryPathElements.Last().Contains(':'))
                {
                    throw new ArgumentException($"The specified paths cannot be combined.");
                }

                // Remove the (first) ".." element, and last base path element.
                relativePathElements.RemoveAt(0);
                baseDirectoryPathElements.RemoveAt(baseDirectoryPathElements.Count - 1);
            }

            var combinedElements = baseDirectoryPathElements.ToList();
            combinedElements.AddRange(relativePathElements);

            var combinedPath = combinedElements.First();

            for (var iNext = 1; iNext < combinedElements.Count; iNext++)
            {
                combinedPath = Path.Combine(combinedPath, combinedElements[iNext]);
            }

            return combinedPath;
        }
    }
}
