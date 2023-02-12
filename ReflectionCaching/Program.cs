using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using ReflectionCaching.Models;
using ReflectionCachingLibrary;

namespace ReflectionCaching
{
    internal class Program
    {
        private static readonly ConcurrentDictionary<string, Tuple<PropertyInfo[], RequiredAttribute[]>> _reflectionCache = new();
        private static readonly ConcurrentDictionary<string, PropertyInfo[]> _reflectionCache2 = new();
        private static readonly long _repetitions = 1_000_000;
        private static List<string>? _messages;

        static void Main()
        {
            Person person = new();
            Stopwatch sw = Stopwatch.StartNew();
            RunRepetitions(ValidateWithoutCache);
            Console.WriteLine("Without Cache:\t\t" + sw.Elapsed.TotalSeconds + "s");
            sw.Restart();
            RunRepetitions(ValidateWithCache_WithAttributes);
            Console.WriteLine("With Cache with attributes:\t" + sw.Elapsed.TotalSeconds + "s");
            sw.Restart();
            RunRepetitions(ValidateWithCache_OnlyProperties);
            Console.WriteLine("With Cache only properties:\t" + sw.Elapsed.TotalSeconds + "s");
        }

        private static void RunRepetitions(Func<object, IEnumerable<string>> method)
        {
            for (int i = 0; i < _repetitions; i++)
            {
                Person person = new();
                person.Validate()
                method(person);
            }
        }

        static IEnumerable<string> ValidateWithoutCache(object obj)
        {

            PropertyInfo[] properties = obj.GetType().GetProperties();
            return GetMissingFieldMessages(obj, properties);
        }
        static IEnumerable<string> ValidateWithCache_OnlyProperties(object obj)
        {
            PropertyInfo[] properties = _reflectionCache2.GetOrAdd(obj.ToString(), o => o.GetType().GetProperties());
            return GetMissingFieldMessages(obj, properties);
        }

        private static List<string> GetMissingFieldMessages(object obj, PropertyInfo[] properties)
        {
            _messages = new();
            foreach (PropertyInfo property in properties)
            {
                RequiredAttribute? requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute != null)
                {
                    object? value = property.GetValue(obj);
                    if (value == null)
                    {
                        _messages.Add($"{property.Name} not set!");
                    }
                }
            }
            return _messages;
        }

        public static List<string> ValidateWithCache_WithAttributes(object obj)
        {
            var messages = new List<string>();
            var cached = _reflectionCache.GetOrAdd(obj.GetType().ToString(), type =>
            {
                var properties = obj.GetType().GetProperties();
                var attributes = new RequiredAttribute[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    var attr = properties[i].GetCustomAttribute<RequiredAttribute>();
                    if (attr != null)
                    {
                        attributes[i] = attr;
                    }
                }

                return Tuple.Create(properties, attributes);
            });

            var properties = cached.Item1;
            var attributes = cached.Item2;
            for (int i = 0; i < properties.Length; i++)
            {
                if (attributes[i] != null)
                {
                    object value = properties[i].GetValue(obj);
                    if (value == null)
                    {
                        messages.Add($"{properties[i].Name} not set!");
                    }
                }
            }
            return messages;
        }
    }
}