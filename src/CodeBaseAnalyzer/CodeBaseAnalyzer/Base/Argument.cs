using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBaseAnalyzer.Base
{
    /// <summary>
    /// Provides runtime checks for arguments.
    /// </summary>
    internal static class Argument
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
