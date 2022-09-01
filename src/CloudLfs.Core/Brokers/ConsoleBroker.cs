using System;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public class ConsoleBroker : IConsoleBroker
    {
        public string? ReadLine() => Console.ReadLine();

        public void WriteLine(string line) => Console.WriteLine(line);
    }
}