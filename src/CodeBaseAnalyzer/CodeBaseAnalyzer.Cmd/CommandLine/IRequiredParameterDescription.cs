
namespace CodeBaseAnalyzer.Cmd.CommandLine
{
    /// <summary>
    /// Describes a required command parameter, which is assigned by order of specification.
    /// </summary>
    public interface IRequiredParameterDescription
    {
        /// <summary>
        /// Gets the command parameter name. Used for informational purposes only.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description. Used to inform users.
        /// </summary>
        string Description { get; }
    }
}
