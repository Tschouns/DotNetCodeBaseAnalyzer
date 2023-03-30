using CodeBaseAnalyzer.Base;

namespace CodeBaseAnalyzer.Cmd.CommandLine.Internal
{
    internal class RequiredParameterDescription : IRequiredParameterDescription
    {
        public RequiredParameterDescription(string name, string description)
        {
            Argument.AssertNotNull(name, nameof(name));
            Argument.AssertNotNull(description, nameof(description));

            this.Name = name;
            this.Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}
