namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public interface IConsoleBroker
    {
        public string? ReadLine();

        public void WriteLine(string line);
    }
}