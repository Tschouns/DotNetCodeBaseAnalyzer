
namespace CodeBaseAnalyzer.Cmd
{
    internal static class ConsoleHelper
    {
        private static readonly object synch = new object();

        public static void WriteLineInColor(ConsoleColor color, string message)
        {
            lock (synch)
            {
                var colorBefore = Console.ForegroundColor;
                Console.ForegroundColor = color;

                Console.WriteLine(message);

                Console.ForegroundColor = colorBefore;
            }
        }
    }
}
