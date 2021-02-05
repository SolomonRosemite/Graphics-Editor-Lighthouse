using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace LighthouseLibrary.Services
{
    public static class UtilService
    {
        private static readonly List<int> Ids = new List<int>();
        private static readonly Random Random = new Random();

        public static int GenerateNewId()
        {
            int value = Random.Next(1, int.MaxValue);

            if (Ids.Contains(value))
                return GenerateNewId();

            Ids.Add(value);

            return value;
        }

        public static FieldInfo GetEventField(this Type type, string eventName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                /* Find events defined as field */
                field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                    break;

                /* Find events defined as property { add; remove; } */
                field = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    break;
                type = type.BaseType;
            }
            return field;
        }

        public static T DeepClone<T>(this T input) where T : ISerializable
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, input);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }

        public static T SpeedCheck<T>(this Func<T> func)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var result = func();
            sw.Stop();

            Console.WriteLine("ElapsedMilliseconds: " + sw.ElapsedMilliseconds);

            return result;
        }
    }
}