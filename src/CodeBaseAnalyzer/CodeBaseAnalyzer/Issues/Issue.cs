using CodeBaseAnalyzer.Base;

namespace CodeBaseAnalyzer.Issues
{
    public class Issue
    {
        private Issue(IssueType type, string message)
        {
            Argument.AssertNotNull(message, nameof(message));

            this.Type = type;
            this.Message = message;
        }

        public IssueType Type { get; }

        public string Message { get; }

        public static Issue Error(string message)
        {
            return new Issue(IssueType.Error, message);
        }

        public static Issue Warn(string message)
        {
            return new Issue(IssueType.Warning, message);
        }

        public override string ToString()
        {
            return $"{this.Type}: {this.Message}";
        }
    }
}
