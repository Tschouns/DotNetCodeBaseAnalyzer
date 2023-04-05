
namespace CodeBaseAnalyzer.Cmd.CommandLine
{
    /// <summary>
    /// Represents a problem which occurred during command execution.
    /// </summary>
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {            
        }
    }
}
