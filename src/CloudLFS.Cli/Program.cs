using System;

namespace CloudLFS.Cli
{
    public class Program
    {
        static void Main(string[] args)
        {
            // by convention all data comes in stdin after client is invoked
            var requestJson = Console.ReadLine();
            
            // todo: deserialize
        }
    }
}