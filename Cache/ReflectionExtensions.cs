using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Cache
{
    public static class ReflectionExtensions
    {
        private static readonly ConcurrentDictionary<string, Tuple<PropertyInfo[], RequiredAttribute[]>> _reflectionCache = new();
        private static List<string>? _messages;

        static void ValidateWithoutCache(object obj)
        {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                GetMissingFieldMessages(obj, properties);
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

        public static List<string> ValidateWithCache(this object obj)
        {
            _messages = new();
            Tuple<PropertyInfo[], RequiredAttribute[]> cached = _reflectionCache.GetOrAdd(obj.GetType().ToString(), type =>
            {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                RequiredAttribute[] attributes = new RequiredAttribute[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    attributes[i] = properties[i].GetCustomAttribute<RequiredAttribute>();
                }

                return Tuple.Create(properties, attributes);
            });

            PropertyInfo[] properties = cached.Item1;
            RequiredAttribute[] attributes = cached.Item2;
            for (int i = 0; i < properties.Length; i++)
            {
                if (attributes[i] != null)
                {
                    object value = properties[i].GetValue(obj);
                    if (value == null)
                    {
                        _messages.Add($"{properties[i].Name} not set!");
                    }
                }
            }

            return _messages;
        }
    }
}