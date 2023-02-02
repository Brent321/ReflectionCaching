using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using ReflectionCaching.Models;

namespace ReflectionCaching
{
    internal class Program
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo[]> _cachedProperties = new();
        private static long _repetitions = 1000000;

        static void Main()
        {
            Person person = new();
            Stopwatch sw = Stopwatch.StartNew();
            ValidateWithoutCache(person);
            Console.WriteLine("Without Cache:\t" + sw.Elapsed.TotalSeconds + "ms");
            sw.Restart();
            ValidateWithCache(person);
            Console.WriteLine("With Cache:\t" + sw.Elapsed.TotalSeconds + "ms");
        }
        static void ValidateWithoutCache(object obj)
        {
            for (int i = 0; i < _repetitions; i++)
            {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                GetMissingFieldMessages(obj, properties);
            }
        }
        
        static void ValidateWithCache(object obj)
        {
            for (int i = 0; i < _repetitions; i++)
            {
                PropertyInfo[] properties = _cachedProperties.GetOrAdd(nameof(obj), o => o.GetType().GetProperties());
                GetMissingFieldMessages(obj, properties);
            }
        }

        private static List<string> GetMissingFieldMessages(object obj, PropertyInfo[] properties)
        {
            var messages = new List<string>();
            foreach (PropertyInfo property in properties)
            {
                RequiredAttribute? requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute != null)
                {
                    object? value = property.GetValue(obj);
                    if (value == null)
                    {
                        messages.Add($"{property.Name} not set!");
                    }
                }
            }
            return messages;
        }
    }
}