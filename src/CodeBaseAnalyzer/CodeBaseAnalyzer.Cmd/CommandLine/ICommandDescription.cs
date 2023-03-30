
namespace CodeBaseAnalyzer.Cmd.CommandLine
{
    /// <summary>
    /// Describes a command which can be executed command line.
    /// </summary>
    public interface ICommandDescription
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the command description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets all required parameter descriptions, in order.
        /// </summary>
        IReadOnlyList<IRequiredParameterDescription> RequiredParameters { get; }

        /// <summary>
        /// Gets all the optional ("named") parameter descriptions (order not relevant).
        /// </summary>
        IReadOnlyList<INamedParameterDescription> NamedParameters { get; }
    }
}
