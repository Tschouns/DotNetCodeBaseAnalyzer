
namespace CodeBaseAnalyzer.Cmd.CommandLine
{
    /// <summary>
    /// Allows to declare required and optional ("named") parameters.
    /// </summary>
    public interface IDeclareParameters
    {
        IDeclareParameters RequiredParameter(string name, string description);
        IDeclareParameters NamedParameter(string name, string shortName, string description);
    }
}
