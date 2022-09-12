using Microsoft.MixedReality.CloudLfs.Brokers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;

namespace CloudLfs.Core.UnitTests
{
    public class TestConsoleBroker : IConsoleBroker
    {
        private Queue<string> _input = new Queue<string>();

        public IReadOnlyList<string> Output => _output;

        private List<string> _output = new List<string>();

        public void SendLine(string value)
        {
            _input.Enqueue(value);
        }

        public string? ReadLine()
        {
            while (_input.Count == 0)
            {
                Thread.Sleep(100);
            }

            return _input.Dequeue();
        }

        public void WriteLine(string line)
        {
            _output.Add(line);
            Console.WriteLine(line);
        }
    }
}