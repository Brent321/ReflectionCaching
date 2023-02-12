using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ValidationExtensions
{
    public static class ExtentionMethods
    {
        private static readonly ConcurrentDictionary<string, Tuple<PropertyInfo[], RequiredAttribute[]>> _reflectionCache = new();
        public static List<string> Validate(this IValidateable obj)
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