
using CodeBaseAnalyzer.Base;

namespace CodeBaseAnalyzer.Cmd.CommandLine.Internal
{
    internal class CommandInternal : IDeclareParameters, ICommandDescription
    {
        private readonly ICommand command;

        private readonly HashSet<string> parameterNames = new HashSet<string>();
        private readonly HashSet<string> parameterShortNames = new HashSet<string>();

        private readonly IList<IRequiredParameterDescription> requiredParameters = new List<IRequiredParameterDescription>();
        private readonly IDictionary<string, INamedParameterDescription> namedParameters = new Dictionary<string, INamedParameterDescription>();

        public CommandInternal(ICommand command, string name, string description)
        {
            Argument.AssertNotNull(command, nameof(command));
            Argument.AssertNotNull(name, nameof(name));
            Argument.AssertNotNull(description, nameof(description));

            this.command = command;

            this.Name = name;
            this.Description = description;
        }

        public string Name { get; }
        public string Description { get; }
        public IReadOnlyList<IRequiredParameterDescription> RequiredParameters => this.requiredParameters.ToList();
        public IReadOnlyList<INamedParameterDescription> NamedParameters => this.namedParameters.Values.ToList();

        public IDeclareParameters RequiredParameter(string name, string description)
        {
            if (this.parameterNames.Contains(name))
            {
                throw new ArgumentException($"A parameter named \"{name}\" is already registered.", nameof(name));
            }

            this.parameterNames.Add(name);
            this.requiredParameters.Add(new RequiredParameterDescription(name, description));

            return this;
        }

        public IDeclareParameters NamedParameter(string name, string shortName, string description)
        {
            if (this.parameterNames.Contains(name))
            {
                throw new ArgumentException($"A parameter named \"{name}\" is already registered.", nameof(name));
            }

            if (this.parameterShortNames.Contains(shortName))
            {
                throw new ArgumentException($"A parameter with short name \"{shortName}\" is already registered.", nameof(shortName));
            }

            this.parameterNames.Add(name);
            this.parameterShortNames.Add(shortName);
            this.namedParameters.Add(name, new NamedParameterDescription(name, shortName, description));

            return this;
        }

        public void Execute(IDictionary<string, string> parametersByName)
        {
            Argument.AssertNotNull(parametersByName, nameof(parametersByName));

            this.command.Execute(parametersByName);
        }
    }
}
