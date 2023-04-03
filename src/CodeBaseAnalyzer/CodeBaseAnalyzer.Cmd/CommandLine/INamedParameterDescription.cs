
namespace CodeBaseAnalyzer.Cmd.CommandLine
{
    /// <summary>
    /// Describes a named (optional) command parameter, consisting of a name (or short name) and value.
    /// </summary>
    public interface INamedParameterDescription
    {
        /// <summary>
        /// Gets the command parameter name (e.g. "directory").
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the short version of the command parameter name (e.g. "d"). Used for more compact specification.
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// Gets the description. Used to inform users.
        /// </summary>
        string Description { get; }
    }
}
