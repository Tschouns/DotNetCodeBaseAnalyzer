
namespace CodeBaseAnalyzer.Cmd.CommandLine
{
    /// <summary>
    /// Knows and executes commands.
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// Registers a command.
        /// </summary>
        /// <param name="command">
        /// The command
        /// </param>
        void RegisterCommand(ICommand command);

        /// <summary>
        /// Gets read-only descriptions of all the known commands.
        /// </summary>
        /// <returns>
        /// A list of all the known commands
        /// </returns>
        IReadOnlyList<ICommandDescription> GetKnownCommands();

        /// <summary>
        /// Executes a command based on the specified command line argumens.
        /// </summary>
        /// <param name="args">
        /// The command line arguments
        /// </param>
        /// <returns>
        /// A return code (0 means no errors)
        /// </returns>
        int Execute(IReadOnlyList<string> args);
    }
}
