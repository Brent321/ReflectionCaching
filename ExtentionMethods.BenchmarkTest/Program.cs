﻿using System.Diagnostics;
using ValidationExtensions;
using ValidationExtensions.Test.Models;

namespace ExtensionMethods.BenchmarkTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var person = new Person();
            var repetitions = 1_000_000;
            for (int i = 0; i < repetitions; i++)
            {
                person.Validate();
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}