using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils
{
    public class KeyValue
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public KeyValue(string key, string value)
        {
            Id = key;
            Key = key;
            Value = value;
        }
    }

    public static class KeyValueExtension
    {
        public static void Add(this List<KeyValue> list, string key, string value)
        => list.Add(new KeyValue(key, value));
    }
}
