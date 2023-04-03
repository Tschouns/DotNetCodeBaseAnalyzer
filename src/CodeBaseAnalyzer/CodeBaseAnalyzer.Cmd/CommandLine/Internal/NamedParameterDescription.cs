
using CodeBaseAnalyzer.Base;

namespace CodeBaseAnalyzer.Cmd.CommandLine.Internal
{
    internal class NamedParameterDescription : INamedParameterDescription
    {
        public NamedParameterDescription(string name, string shortName, string description)
        {
            Argument.AssertNotNull(name, nameof(name));
            Argument.AssertNotNull(shortName, nameof(shortName));
            Argument.AssertNotNull(description, nameof(description));

            this.Name = name;
            this.ShortName = shortName;
            this.Description = description;
        }

        public string Name { get; }
        public string ShortName { get; }
        public string Description { get; }
    }
}
