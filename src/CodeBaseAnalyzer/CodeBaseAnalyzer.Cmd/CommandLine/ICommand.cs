
namespace CodeBaseAnalyzer.Cmd.CommandLine
{
    /// <summary>
    /// Represents a command which can be executed command line.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        string GetName();

        /// <summary>
        /// Gets the command description.
        /// </summary>
        string GetDescription();

        /// <summary>
        /// Has the command declare it's required and optional ("named") parameters.
        /// </summary>
        /// <param name="declare">
        /// The declaration interface
        /// </param>
        void DeclareParameters(IDeclareParameters declare);

        /// <summary>
        /// Executes the command given the specified parameters.
        /// </summary>
        /// <param name="parametersByName">
        /// The parameters dictionalry, organized by parameter name
        /// </param>
        void Execute(IDictionary<string, string> parametersByName);
    }
}
