namespace CodeBaseAnalyzer.Base
{
    /// <summary>
    /// Provides runtime checks for arguments.
    /// </summary>
    public static class Argument
    {
        /// <summary>
        /// Asserts that the specified argument be not null.
        /// </summary>
        /// <param name="argument">
        /// The argument to check
        /// </param>
        /// <param name="argumentName">
        /// The argument name
        /// </param>
        public static void AssertNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
